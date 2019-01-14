module BusinessLogic

open MF.ConsoleStyle
open AccountCreation
open AccountCreation.Domain

// =============================
// Types for actions executed in console application
// =============================

type UserInput = string
type Message = string

and ConsoleResult =
    | Success of Message
    | Error of Message

type ConsoleAction1 =
    UserInput -> ConsoleResult

type ConsoleAction2 =
    UserInput -> UserInput -> ConsoleResult

// =============================
// Implementation
// =============================

let printResult = function
    | Success message ->
        Console.success message
        0
    | Error message ->
        Console.error message
        1

let action1 : ConsoleAction1 =
    fun input ->
        Console.section "Action 1"

        input
        |> UnvalidatedEmail
        |> ConsoleApi.action1
        |> sprintf "Action 1 ends up with unconfirmed account %A"
        |> Success

let action2 : ConsoleAction2 =
    fun emailInput codeInput ->
        Console.section "Action 2"
        let response =
            {
                Email = emailInput
                Code = codeInput
            }
            |> ConsoleApi.action2 Console.ask

        Console.successf "Action 2 ends up with Activate account response %A" response

        Console.section "Action 3"
        match response |> ConsoleApi.action3 with
        | Some activeAcount ->
            activeAcount
            |> sprintf "Action 3 ends up with active account %A"
            |> Success
        | None ->
            "Action 3 ends up without creating an account"
            |> Success
