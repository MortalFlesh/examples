module internal AccountCreation.ImplementationWithoutEffects

open AccountCreation.Common
open AccountCreation.Domain

// ======================================================
// This file contains the implementation for the AccountCreation workflows
// WITHOUT any effects like Result or Async
//
// There are two parts:
// * the first section contains the (type-only) definitions for each step
// * the second section contains the implementations for each step
//   and the implementation of the overall workflow
// ======================================================

// ------------------------------------
// the workflow itself, without effects

type CreateUnconfirmedAccountWithoutEffects =
    UnvalidatedEmail -> UnconfirmedAccount

type ConfirmAccountWithoutEffects =
    UnvalidatedInactiveAccount -> ConfirmedAccount

type AskUserToActiveAccountWithoutEffects =
    ConfirmedAccount -> ActivateAccountResponse

type ActivateAccountWithoutEffects =
    ActivateAccountResponse -> ActiveAccount

// ======================================================
// Override the SimpleType constructors
// so that they raise exceptions rather than return Results
// ======================================================

let failOnError result =
    match result with
    | Ok success -> success
    | Error error -> failwithf "%A" error

module String20to50 =
    let create fieldName = String20to50.create fieldName >> failOnError

module EmailAddress =
    let create = EmailAddress.create >> failOnError

module Name =
    let create = Name.create >> failOnError

// ======================================================
// Action 1 / Section 1 : Define each step in the workflow using types
// ======================================================

// ---------------------------
// Validation step

type ValidateEmail =
    IsUnique                // dependency
        -> UnvalidatedEmail // input
        -> ValidEmail       // output

and IsUnique =
    EmailAddress -> bool

// ---------------------------
// Confirmation code creation step

type CreateConfirmationCode =
    ValidEmail -> ConfirmationCode

// ---------------------------
// Uncofirmed account creation step

type CreateUnconfirmedAccount =
    ValidEmail -> ConfirmationCode -> UnconfirmedAccount

// ---------------------------
// Confirmation email step

type SendConfirmationEmail =
    UnconfirmedAccount -> SendResult

and SendResult =
    | Sent
    | NotSent

// ======================================================
// Action 1 / Section 2 : Implementation steps
// ======================================================

// ---------------------------
// Validation step

let validateEmail : ValidateEmail =
    fun isUnique (UnvalidatedEmail unvalidatedEmail) ->
        let emailAddress =
            unvalidatedEmail
            |> EmailAddress.create

        if not (emailAddress |> isUnique) then
            "E-mail is already used."
            |> ValidationError
            |> NotUnique
            |> EmailValidationFailed
            |> failwithf "%A"

        ValidEmail emailAddress

// ---------------------------
// Confirmation code creation step

let createConfirmationCode : CreateConfirmationCode =
    fun (ValidEmail email) ->
        email
        |> Code.create
        |> ConfirmationCode

// ---------------------------
// Uncofirmed account creation step

let createUnconfirmedAccount : CreateUnconfirmedAccount =
    fun validEmail confirmationCode ->
        {
            Email = validEmail
            ConfirmationCode = confirmationCode
        }

// ---------------------------
// Confirmation email step

let sendConfirmationEmail : SendConfirmationEmail =
    fun uncofirmedAccount ->
        let { Email = validEmail; ConfirmationCode = confirmationCode } = uncofirmedAccount

        let (ValidEmail emailAddress) = validEmail
        let emailValue = emailAddress |> EmailAddress.value

        let (ConfirmationCode code) = confirmationCode
        let codeValue = code |> Code.value

        printfn "Email body: code for e-mail %s is %s" emailValue codeValue     // sending mail sideeffect
        Sent

// =========================
// Action 1 workflow
// =========================

let action1
    validateEmail               // dependency
    createConfirmationCode      // dependency
    createUnconfirmedAccount    // dependency
    sendConfirmationEmail       // dependency
    : CreateUnconfirmedAccountWithoutEffects =  // function definition

    fun unvalidatedEmail ->
        let validEmail =
            unvalidatedEmail
            |> validateEmail
        let confirmationCode =
            validEmail
            |> createConfirmationCode
        let unconfirmedAccount =
            validEmail
            |> createUnconfirmedAccount confirmationCode
        let sendResult =
            unconfirmedAccount
            |> sendConfirmationEmail

        match sendResult with
        | NotSent -> ConfirmationEmailNotSent |> failwithf "%A"
        | Sent -> validEmail
