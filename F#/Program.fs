// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    Example.Mailbox.run()
    
    0 // return an integer exit code
