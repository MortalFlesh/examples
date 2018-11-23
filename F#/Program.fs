// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    // Example.Mailbox.run()
    Example.MonadicAdd.computeStandardly()
    Example.MonadicAdd.computeAplicatively()
    Example.MonadicAdd.coomputeByComputedExpressions()

    0 // return an integer exit code
