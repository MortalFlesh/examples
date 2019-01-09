// Learn more about F# at http://fsharp.org

open System
open MF.ConsoleStyle
open CompanyDomain

[<EntryPoint>]
let main argv =
    Console.title "Hello from DDD Error handling"

    let mutable shouldRun = true
    while shouldRun do
        let rawMathematician = Console.ask "Who are you?"
        Console.section "Please, provide:"

        {
            Mathematician = rawMathematician
            Divider = Console.ask "Divider:"
            Divisior = Console.ask "Divisior:"
        }
        |> DivideInts.byUserInput Console.message
        |> Console.messagef "---\nResult:\n%A"

        match Console.ask "Would you like to continue?" with
        | "yes"
        | "y"
        | "sure" -> ()
        | _ ->
            Console.message "Thank you for using our service!"
            shouldRun <- false

    0
