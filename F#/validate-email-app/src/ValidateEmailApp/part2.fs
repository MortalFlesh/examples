module BusinessLogic

// Whats new?
// ==========

open System
open MF.ConsoleStyle
open AccountCreation
open MF.ConsoleStyle

let action1 input =
    Console.section "Action 1"

    input
    |> ConsoleApi.action1
    |> Console.successf "Action 1 ends up with unconfirmed account %A"

let action2 email code =
    Console.section "Action 2"
    failwithf "Not implemented yet"

let action3 email =
    Console.section "Action 3"
    failwithf "Not implemented yet"
