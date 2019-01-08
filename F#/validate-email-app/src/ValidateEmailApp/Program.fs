// Learn more about F# at http://fsharp.org

open System
open MF.ConsoleStyle

[<EntryPoint>]
let main argv =
    Console.title "Hello from Validate E-mail App"

    match argv with
    | [| "action1"; input |] ->
        Console.section "Action 1"

        let code = Guid.NewGuid()
        Console.successf2 "Email body: code for e-mail %s is %s" input (code.ToString())
        0
    | [| "action2"; email; code |] ->
        Console.section "Action 2"

        match Console.ask "Do you want to create an account?" with
        | "yes"
        | "y" ->
            Console.section "Action 3"
            let name = Console.ask "Type in your name, please:"

            Console.successf2 "Account is created for email %s with name %s" email name
            0
        | _ ->
            Console.success "Account is not created"
            0
    | _ ->
        Console.error "Invalid or unknown action"
        0
