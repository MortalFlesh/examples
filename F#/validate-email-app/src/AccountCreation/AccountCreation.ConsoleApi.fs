module AccountCreation.ConsoleApi

open AccountCreation.Common
open AccountCreation.Domain
open AccountCreation

// todo define type for console<action1>, ...

// =============================
// Implementation
// =============================

// setup "dummy" dependencies

let isUnique : ImplementationWithoutEffects.IsUnique =
    fun email ->
        match email |> EmailAddress.value with
        | "already@there.cz" -> false
        | _ -> true

let sendMail : ImplementationWithoutEffects.SendMail =
    fun (EmailBody emailBody) ->
        printfn "Email body: %s" emailBody
        ImplementationWithoutEffects.Sent

let createUnconfirmedAccount =
    ImplementationWithoutEffects.createUnconfirmedAccount

let checkConfirmationCode : ImplementationWithoutEffects.CheckConfirmationCode =
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

let action1 unvalidatedEmail =
    // inject dependencies
    let action1Workflow =
        ImplementationWithoutEffects.action1
            (ImplementationWithoutEffects.validateEmail isUnique)
            ImplementationWithoutEffects.createConfirmationCode
            createUnconfirmedAccount
            (ImplementationWithoutEffects.sendConfirmationEmail sendMail)

    unvalidatedEmail
    |> action1Workflow

let action2 askUser unvalidatedUnconfirmedAccount =
    let askUser = askUserQuestion askUser

    // inject dependencies
    let action2workflow =
        ImplementationWithoutEffects.action2
            (ImplementationWithoutEffects.validateUnconfirmedAccount createUnconfirmedAccount)
            (ImplementationWithoutEffects.confirmUnconfirmedAccount checkConfirmationCode)
            (ImplementationWithoutEffects.askUserToActivateAccountWithoutEffects askUser)

    unvalidatedUnconfirmedAccount
    |> action2workflow
