module AccountCreation.ConsoleApi

open AccountCreation.Common
open AccountCreation.Domain
open AccountCreation

// =============================
// Types for actions executed in console application
// =============================

type ConsoleAction1 =
    UnvalidatedEmail -> UnconfirmedAccount

type ConsoleAction2 =
    UnvalidatedUnconfirmedAccount -> ActivateAccountResponse

type ConsoleAction3 =
    ActivateAccountResponse -> ActiveAccount option

// =============================
// Implementation
// =============================

// setup "dummy" dependencies

let isUnique : Implementation.IsUnique =
    fun email ->
        match email |> EmailAddress.value with
        | "already@there.cz" -> false
        | _ -> true

let sendMail : Implementation.SendMail =
    fun (EmailBody emailBody) ->
        printfn "Email body: %s" emailBody
        Implementation.Sent

let createUnconfirmedAccount =
    Implementation.createUnconfirmedAccount

let checkConfirmationCode : Implementation.CheckConfirmationCode =
    fun (ValidEmail email) (ConfirmationCode code) ->
        code
        |> Code.value
        |> fun codeValue ->
            codeValue.Split('|')
            |> Array.head
            |> (=) (email |> EmailAddress.value)

let askUserQuestion consoleAsk : ImplementationWithoutEffects.AskUser =
    fun (ImplementationWithoutEffects.Question question) ->
        question
        |> consoleAsk

// -------------------------------
// workflow
// -------------------------------

let private failWithError = function    // todo - remove this helper
    | Ok success -> success
    | Error error -> failwithf "Unhandled error: %A" error

let action1 : ConsoleAction1 =
    fun unvalidatedEmail ->
        // inject dependencies
        let action1Workflow =
            Implementation.action1
                (Implementation.validateEmail isUnique)
                Implementation.createConfirmationCode
                createUnconfirmedAccount
                (Implementation.sendConfirmationEmail sendMail)

        unvalidatedEmail
        |> action1Workflow
        |> failWithError

let action2
    askUser
    : ConsoleAction2 =

    fun unvalidatedUnconfirmedAccount ->
        let askUser = askUserQuestion askUser

        // inject dependencies
        let action2workflow =
            ImplementationWithoutEffects.action2
                (ImplementationWithoutEffects.validateUnconfirmedAccount createUnconfirmedAccount)
                (ImplementationWithoutEffects.confirmUnconfirmedAccount checkConfirmationCode)
                (ImplementationWithoutEffects.askUserToActivateAccountWithoutEffects askUser)

        unvalidatedUnconfirmedAccount
        |> action2workflow

let action3 : ConsoleAction3 =
    fun response ->
        // inject dependencies
        let action3workflow =
            ImplementationWithoutEffects.action3
                ImplementationWithoutEffects.createActiveAccount

        response
        |> action3workflow
