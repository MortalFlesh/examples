module internal AccountCreation.ImplementationWithoutEffects

open System
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
    UnvalidatedUnconfirmedAccount -> ActivateAccountResponse

type ActivateAccountWithoutEffects =
    ActivateAccountResponse -> ActiveAccount option

// ======================================================
// Override the SimpleType constructors
// so that they raise exceptions rather than return Results
// ======================================================

let failOnError result =
    match result with
    | Ok success -> success
    | Error error -> failwithf "%A" error

module String20to50 =
    let create fieldName = String2to50.create fieldName >> failOnError

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
    SendMail -> UnconfirmedAccount -> SendResult

and SendMail =
    EmailBody -> SendResult

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
    fun sendMail uncofirmedAccount ->
        let { Email = validEmail; ConfirmationCode = confirmationCode } = uncofirmedAccount

        let (ValidEmail emailAddress) = validEmail
        let emailValue = emailAddress |> EmailAddress.value

        let (ConfirmationCode code) = confirmationCode
        let codeValue = code |> Code.value

        sprintf "code for e-mail %s is %s" emailValue codeValue
        |> EmailBody
        |> sendMail

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
            confirmationCode
            |> createUnconfirmedAccount validEmail
        let sendResult =
            unconfirmedAccount
            |> sendConfirmationEmail

        match sendResult with
        | NotSent -> ConfirmationEmailNotSent |> failwithf "%A"
        | Sent -> unconfirmedAccount

// ======================================================
// Action 2 / Section 1 : Define each step in the workflow using types
// ======================================================

// ---------------------------
// Validation step

type ValidateUnconfirmedAccount =
    CreateUnconfirmedAccount -> UnvalidatedUnconfirmedAccount -> UnconfirmedAccount

// ---------------------------
// Confirmed account creation step

type ConfirmUnconfirmedAccount =
    CheckConfirmationCode -> UnconfirmedAccount -> ConfirmedAccount

and CheckConfirmationCode =
    ValidEmail -> ConfirmationCode -> bool

// ---------------------------
// Ask user to activate account step

type AskUserToActivateAccountWithoutEffects =
    AskUser -> ConfirmedAccount -> ActivateAccountResponse

and AskUser =
    Question -> string

and Question = Question of string

// ======================================================
// Action 2 / Section 2 : Implementation steps
// ======================================================

// ---------------------------
// Validation step

let validateUnconfirmedAccount : ValidateUnconfirmedAccount =
    fun createUnconfirmedAccount unconfirmedAccount ->
        if String.IsNullOrEmpty(unconfirmedAccount.Email) || String.IsNullOrEmpty(unconfirmedAccount.Code) then
            failwithf "%A" InvalidInactiveAccount

        let validEmail = unconfirmedAccount.Email |> EmailAddress.create |> ValidEmail
        let confirmationCode = unconfirmedAccount.Code |> Code.fromGenerated |> ConfirmationCode

        createUnconfirmedAccount validEmail confirmationCode

// ---------------------------
// Confirmed account creation step

let confirmUnconfirmedAccount : ConfirmUnconfirmedAccount =
    fun checkConfirmationCode unconfirmedAccount ->
        if checkConfirmationCode unconfirmedAccount.Email unconfirmedAccount.ConfirmationCode |> not then
            failwithf "%A" WrongEmailCodeCombination

        {
            Email = unconfirmedAccount.Email |> ActiveEmail
        }

// ---------------------------
// Ask user to activate account step

let askUserToActivateAccountWithoutEffects : AskUserToActivateAccountWithoutEffects =
    fun askUser confirmedAccount ->
        let shouldCreateAccount =
            confirmedAccount.Email
            |> sprintf "Do you want to create an account for %A? [yes | no]"
            |> Question
            |> askUser
        match shouldCreateAccount with
        | "no" -> No
        | "yes" ->
            let name =
                "Type in your name:"
                |> Question
                |> askUser
                |> UnvalidatedName

            Yes (confirmedAccount.Email, name)

        // How to handle incomplete pattern here? Should we take all other than "no" as "yes"? Or we should fail?
        | _ -> failwithf "Wrong answer given - only \"yes\" and \"no\" are allowed."

// =========================
// Action 2 workflow
// =========================

let action2
    validateUnconfirmedAccount          // dependency
    confirmUnconfirmedAccount           // dependency
    askUserToActiveAccount              // dependency
    : ConfirmAccountWithoutEffects =    // function definition

    fun unvalidatedUncornfirmedAccount ->
        let unconfirmedAccount =
            unvalidatedUncornfirmedAccount
            |> validateUnconfirmedAccount

        unconfirmedAccount
        |> confirmUnconfirmedAccount
        |> askUserToActiveAccount

// ======================================================
// Action 3 / Section 1 : Define each step in the workflow using types
// ======================================================

// ---------------------------
// Creation of active account step

type CreateActiveAccount =
    ActiveEmail -> UserName -> ActiveAccount

// ======================================================
// Action 3 / Section 2 : Implementation steps
// ======================================================

let createActiveAccount : CreateActiveAccount =
    fun activeEmail userName ->
        {
            Email = activeEmail
            Name = userName
        }

// =========================
// Action 3 workflow
// =========================

let action3
    createActiveAccount
    : ActivateAccountWithoutEffects =

    fun activateAccountResponse ->
        match activateAccountResponse with
        | No -> None
        | Yes (activeEmail, UnvalidatedName unvalidatedName) ->
            unvalidatedName
            |> Name.create
            |> UserName
            |> createActiveAccount activeEmail
            |> Some
