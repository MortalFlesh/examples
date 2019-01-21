module internal AccountCreation.Implementation

open System
open AccountCreation.Common
open AccountCreation.Domain

// ======================================================
// This file contains the final implementation for the AccountCreation workflows
//
// There are two parts:
// * the first section contains the (type-only) definitions for each step
// * the second section contains the implementations for each step
//   and the implementation of the overall workflow
// ======================================================

// ======================================================
// Action 1 / Section 1 : Define each step in the workflow using types
// ======================================================

// ---------------------------
// Validation step

type ValidateEmail =
    IsUnique                // dependency
        -> UnvalidatedEmail // input
        -> Result<ValidEmail, EmailValidationError>       // output

and IsUnique =
    EmailAddress -> bool    // todo - is it really that simple?

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
    SendMail -> UnconfirmedAccount -> SendResult    // todo - will it be immediate?

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
        result {
            let! emailAddress =
                unvalidatedEmail
                |> EmailAddress.create
                |> Result.mapError WrongEmailAddress

            if not (emailAddress |> isUnique) then
                return!
                    "E-mail is already used."
                    |> ValidationError
                    |> NotUnique
                    |> Error

            return ValidEmail emailAddress
        }

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
    : Action.CreateUnconfirmedAccount =  // function definition

    fun unvalidatedEmail ->
        result {
            let! validEmail =
                unvalidatedEmail
                |> validateEmail
                |> Result.mapError EmailValidationFailed
            let confirmationCode =
                validEmail
                |> createConfirmationCode
            let unconfirmedAccount =
                confirmationCode
                |> createUnconfirmedAccount validEmail
            let sendResult =
                unconfirmedAccount
                |> sendConfirmationEmail

            return!
                match sendResult with
                | Sent -> Ok unconfirmedAccount
                | NotSent -> Error ConfirmationEmailNotSent
        }

// ======================================================
// Action 2 / Section 1 : Define each step in the workflow using types
// ======================================================

// ---------------------------
// Validation step

type ValidateUnconfirmedAccount =
    CreateUnconfirmedAccount
        -> UnvalidatedUnconfirmedAccount
        -> Result<UnconfirmedAccount, ConfirmationError>

// ---------------------------
// Confirmed account creation step

type ConfirmUnconfirmedAccount =
    CheckConfirmationCode
        -> UnconfirmedAccount
        -> Result<ConfirmedAccount, ConfirmationError>

and CheckConfirmationCode =
    ValidEmail -> ConfirmationCode -> bool

// ---------------------------
// Ask user to activate account step

type AskUserToActivateAccount =
    AskUser
        -> ConfirmedAccount
        -> Result<ActivateAccountResponse, WrongAnswerError>

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
        result {
            if String.IsNullOrEmpty(unconfirmedAccount.Email) || String.IsNullOrEmpty(unconfirmedAccount.Code) then
                return! Error InvalidInactiveAccount

            let! emailAddress =
                unconfirmedAccount.Email
                |> EmailAddress.create
                |> Result.mapError (fun _ -> InvalidInactiveAccount)

            let validEmail = ValidEmail emailAddress
            let confirmationCode = unconfirmedAccount.Code |> Code.fromGenerated |> ConfirmationCode

            return createUnconfirmedAccount validEmail confirmationCode
        }

// ---------------------------
// Confirmed account creation step

let confirmUnconfirmedAccount : ConfirmUnconfirmedAccount =
    fun checkConfirmationCode unconfirmedAccount ->
        result {
            if checkConfirmationCode unconfirmedAccount.Email unconfirmedAccount.ConfirmationCode |> not then
                return! Error WrongEmailCodeCombination

            return { Email = unconfirmedAccount.Email |> ActiveEmail }
        }

// ---------------------------
// Ask user to activate account step

let askUserToActivateAccount : AskUserToActivateAccount =
    fun askUser confirmedAccount ->
        result {
            let shouldCreateAccount =
                confirmedAccount.Email
                |> sprintf "Do you want to create an account for %A? [yes | no]"
                |> Question
                |> askUser

            return!
                match shouldCreateAccount with
                | "no" -> Ok No
                | "yes" ->
                    let name =
                        "Type in your name:"
                        |> Question
                        |> askUser
                        |> UnvalidatedName

                    Yes (confirmedAccount.Email, name)
                    |> Ok
                | otherAnswer -> Error (WrongAnswerError otherAnswer)
        }

// =========================
// Action 2 workflow
// =========================

let action2
    validateUnconfirmedAccount   // dependency
    confirmUnconfirmedAccount    // dependency
    askUserToActiveAccount       // dependency
    : Action.ConfirmAccount =    // function definition

    fun unvalidatedUncornfirmedAccount ->
        result {
            let! unconfirmedAccount =
                unvalidatedUncornfirmedAccount
                |> validateUnconfirmedAccount
                |> Result.mapError ConfirmationError

            let! confirmedAccount =
                unconfirmedAccount
                |> confirmUnconfirmedAccount
                |> Result.mapError ConfirmationError

            return!
                confirmedAccount
                |> askUserToActiveAccount
                |> Result.mapError WrongAnswer
        }

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

//let action3
//    createActiveAccount
//    : Action.ActivateAccount =
//
//    fun activateAccountResponse ->
//        match activateAccountResponse with
//        | No -> None
//        | Yes (activeEmail, UnvalidatedName unvalidatedName) ->
//            unvalidatedName
//            |> Name.create
//            |> UserName
//            |> createActiveAccount activeEmail
//            |> Some
//