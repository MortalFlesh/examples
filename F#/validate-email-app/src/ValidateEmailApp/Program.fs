// Learn more about F# at http://fsharp.org

open System
open MF.ConsoleStyle

[<EntryPoint>]
let main argv =
    Console.title "Hello from Validate E-mail App"

    try
        match argv with
        | [| "action1"; input |] ->
            input
            |> BusinessLogic.action1
            0
        | [| "action2"; email; code |] ->
            BusinessLogic.action2 email code
            |> BusinessLogic.action3
            0
        | _ ->
            Console.error "Invalid or unknown action"
            0
    with
    | ex ->
        Console.errorf "%s" ex.Message
        1
