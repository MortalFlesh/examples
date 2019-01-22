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
        -> AsyncResult<ValidEmail, EmailValidationError>       // output

and IsUnique =
    EmailAddress -> Async<bool>

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
    SendMail
        -> UnconfirmedAccount
        -> Async<SendResult>

and SendMail =
    EmailBody -> AsyncResult<unit, SendingEmailError>

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
        asyncResult {
            let! emailAddress =
                unvalidatedEmail
                |> EmailAddress.create
                |> Result.mapError WrongEmailAddress
                |> AsyncResult.ofResult

            let! isEmailUnique =
                emailAddress
                |> isUnique
                |> AsyncResult.ofAsync

            if not isEmailUnique then
                return!
                    "E-mail is already used."
                    |> ValidationError
                    |> NotUnique
                    |> Error
                    |> AsyncResult.ofResult

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

let sendConfirmationEmail log : SendConfirmationEmail =
    fun sendMail uncofirmedAccount ->
        async {
            let { Email = validEmail; ConfirmationCode = confirmationCode } = uncofirmedAccount

            let (ValidEmail emailAddress) = validEmail
            let emailValue = emailAddress |> EmailAddress.value

            let (ConfirmationCode code) = confirmationCode
            let codeValue = code |> Code.value

            let! sentEmailResult =
                sprintf "code for e-mail %s is %s" emailValue codeValue
                |> EmailBody
                |> sendMail

            return
                match sentEmailResult with
                | Error error ->
                    // we dont need to handle error now, just return, that is was NOT SENT
                    // and log the error
                    sprintf "%A" error |> log
                    NotSent
                | _ -> Sent
        }

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
        asyncResult {
            let! validEmail =
                unvalidatedEmail
                |> validateEmail
                |> AsyncResult.mapError EmailValidationFailed
            let confirmationCode =
                validEmail
                |> createConfirmationCode
            let unconfirmedAccount =
                confirmationCode
                |> createUnconfirmedAccount validEmail
            let! sendResult =
                unconfirmedAccount
                |> sendConfirmationEmail
                |> AsyncResult.ofAsync

            return!
                match sendResult with
                | Sent -> Ok unconfirmedAccount
                | NotSent -> Error ConfirmationEmailNotSent
                |> AsyncResult.ofResult
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

let action3
    createActiveAccount
    : Action.ActivateAccount =

    fun activateAccountResponse ->
        result {
            match activateAccountResponse with
            | No -> return None
            | Yes (activeEmail, UnvalidatedName unvalidatedName) ->
                let! name =
                    unvalidatedName
                    |> Name.create
                    |> Result.mapError WrongName

                return
                    name
                    |> UserName
                    |> createActiveAccount activeEmail
                    |> Some
        }
