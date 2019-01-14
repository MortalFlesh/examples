module BusinessLogic

// Whats new?
// ==========

open System
open MF.ConsoleStyle
open AccountCreation
open AccountCreation.Domain

let action1 input =
    Console.section "Action 1"

    input
    |> UnvalidatedEmail
    |> ConsoleApi.action1
    |> Console.successf "Action 1 ends up with unconfirmed account %A"

let action2 email code =
    Console.section "Action 2"

    {
        Email = email
        Code = code
    }
    |> ConsoleApi.action2 Console.ask
    |> fun response ->
        Console.successf "Action 2 ends up with Activate account response %A" response
        response

let action3 email =
    Console.section "Action 3"
    failwithf "Not implemented yet"
