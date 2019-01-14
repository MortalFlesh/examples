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

let action3 response =
    Console.section "Action 3"

    match response |> ConsoleApi.action3 with
    | Some activeAcount ->
        activeAcount
        |> Console.successf "Action 3 ends up with active account %A"
    | None ->
        Console.success "Action 3 ends up without creating an account"
