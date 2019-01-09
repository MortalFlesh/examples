module BusinessLogic

// Whats new?
// ==========

open System
open MF.ConsoleStyle
open AccountCreation.Types

let action1 input =
    Console.section "Action 1"

    let code = Guid.NewGuid()

    input + "|" + code.ToString()
    |> Console.successf2 "Email body: code for e-mail %s is %s" input

let action2 email code =
    Console.section "Action 2"

    match Console.ask "Do you want to create an account?" with
    | "yes"
    | "y" -> true
    | _ -> false

let action3 email =
    Console.section "Action 3"

    let name = Console.ask "Type in your name, please:"
    let account = {Email = email; Name = name}

    Console.successf2 "Account is created for email %s with name %s" account.Email account.Name