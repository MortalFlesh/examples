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

// -------------------------------
// workflow
// -------------------------------

let action1 input =
    // inject dependencies
    let action1Workflow =
        ImplementationWithoutEffects.action1
            (ImplementationWithoutEffects.validateEmail isUnique)
            ImplementationWithoutEffects.createConfirmationCode
            ImplementationWithoutEffects.createUnconfirmedAccount
            (ImplementationWithoutEffects.sendConfirmationEmail sendMail)

    input
    |> UnvalidatedEmail
    |> action1Workflow
