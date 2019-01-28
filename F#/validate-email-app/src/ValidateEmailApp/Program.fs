// Learn more about F# at http://fsharp.org

open System
open MF.ConsoleStyle
open AccountCreation

[<EntryPoint>]
let main argv =
    // Example.ErrorHandlingExample.errorHandlingExample()

    Console.title "Hello from Validate E-mail App"

    let printResult = function
        | ConsoleApi.Success message ->
            Console.success message
            0
        | ConsoleApi.Error message ->
            Console.error message
            1

    try
        match argv with
        | [| "action1"; input |] ->
            async {
                return!
                    input
                    |> ConsoleApi.createUnconfirmedAccountAction Console.section (Console.errorf "[log] %s")
            }
            |> Async.RunSynchronously
            |> printResult
        | [| "action2"; email; code |] ->
            (email, code)
            |> ConsoleApi.confirmAndTryToActivateAccountAction Console.section Console.ask
            |> printResult
        | _ ->
            Console.error "Invalid or unknown action"
            0
    with
    | ex ->
        Console.errorf "%s" ex.Message
        1
