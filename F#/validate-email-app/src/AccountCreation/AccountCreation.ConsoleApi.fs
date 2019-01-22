module AccountCreation.ConsoleApi

open AccountCreation.Common
open AccountCreation.Domain
open AccountCreation

// =============================
// Types for actions executed in console application
// =============================

type UserInput = string
type Message = string

and ConsoleResult =
    | Success of Message
    | Error of Message

type Action2or3Error =
    | Action2Error of ActivationFailed
    | Action3Error of ActivationFailed

[<RequireQualifiedAccess>]
module ConsoleAction =
    type CreateUnconfirmedAccount =
        UserInput -> ConsoleResult

    type ConfirmAndTryToActivateAccount =
        (UserInput * UserInput) -> ConsoleResult

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

let askUserQuestion consoleAsk : Implementation.AskUser =
    fun (Implementation.Question question) ->
        question
        |> consoleAsk

// -------------------------------
// workflow
// -------------------------------

let createUnconfirmedAccountAction
    consoleSection
    : ConsoleAction.CreateUnconfirmedAccount =

    fun userInput ->
        consoleSection "Action 1 - create unconfirmed account"

        // inject dependencies
        let action1Workflow =
            Implementation.action1
                (Implementation.validateEmail isUnique)
                Implementation.createConfirmationCode
                createUnconfirmedAccount
                (Implementation.sendConfirmationEmail sendMail)

        userInput
        |> UnvalidatedEmail
        |> action1Workflow
        |> function
            | Result.Ok unconfirmedAccount ->
                unconfirmedAccount
                |> sprintf "Action 1 ends up with unconfirmed account %A"
                |> ConsoleResult.Success
            | Result.Error error ->
                error
                |> sprintf "Action 1 ends up with error:\n%A"
                |> ConsoleResult.Error

let confirmAndTryToActivateAccountAction
    consoleSection
    consoleAsk
    : ConsoleAction.ConfirmAndTryToActivateAccount =

    fun (emailInput, codeInput) ->
        result {
            consoleSection "Action 2 - Confirm account"

            // inject dependencies
            let askUser = askUserQuestion consoleAsk
            let action2workflow =
                Implementation.action2
                    (Implementation.validateUnconfirmedAccount createUnconfirmedAccount)
                    (Implementation.confirmUnconfirmedAccount checkConfirmationCode)
                    (Implementation.askUserToActivateAccount askUser)

            let unvalidatedUnconfirmedAccount = {
                Email = emailInput
                Code = codeInput
            }

            let! response =
                unvalidatedUnconfirmedAccount
                |> action2workflow
                |> Result.mapError Action2Error

            let action2SuccessMessage = sprintf "Action 2 ends up with Activate account response %A" response

            consoleSection "Action 3 - Activate account (optional)"

            // inject dependencies
            let action3workflow =
                Implementation.action3
                    Implementation.createActiveAccount

            let! activeAccountOption =
                response
                |> action3workflow
                |> Result.mapError Action3Error

            let action3SuccessMessage =
                match activeAccountOption with
                | Some activeAccount -> sprintf "Action 3 ends up with active account %A" activeAccount
                | _ -> "Action 3 ends up without creating an account"

            return [action2SuccessMessage; action3SuccessMessage]
        }
        |> function
            | Result.Ok successMessages ->
                successMessages
                |> String.concat "\n"
                |> ConsoleResult.Success
            | Result.Error action2or3Error ->
                match action2or3Error with
                | Action2Error error -> error |> sprintf "Actions 2 ends up with error:\n%A"
                | Action3Error error -> error |> sprintf "Actions 3 ends up with error:\n%A"
                |> ConsoleResult.Error
