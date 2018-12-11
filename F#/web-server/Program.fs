open System
open System.Threading
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

module State =
    open System.Collections.Concurrent

    let private current = new ConcurrentDictionary<string, int>()
    current.["current"] <- 0

    let getCurrent() =
        sprintf "Current: %i" current.["current"]

    let increment a =
        current.AddOrUpdate (
            "current",
            1,
            fun _ oldState -> oldState + 1
        ) |> ignore
        a

[<EntryPoint>]
let main argv =
    let getState source _ =
        printfn "getting state from %s ..." source
        State.getCurrent()

    let renderCurrent source () =
        printfn "%s" (getState source ())

    let incrementing =
        async {
            for _ in 0 .. 20 do
                do! Async.Sleep 1000

                State.increment ()
                |> renderCurrent "incrementing"
            printfn "incrementing finished"
        }

    let cts = new CancellationTokenSource()
    let server =
        choose [
            GET >=> choose [
                path "/"
                    >=> OK "Hello from server"
                path "/current"
                    >=> request (getState "server" >> OK)
            ]
        ]
        |> startWebServerAsync { defaultConfig with cancellationToken = cts.Token }
        |> snd

    let killServerAfter timeout =
        async {
            do! Async.Sleep timeout
            printfn "Ending server..."
            cts.Cancel()
            printfn "Cancellation complete ..."
        }

    printfn "Running ..."
    Async.Start (server, cts.Token)

    [
        incrementing
        killServerAfter (30 * 1000)
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> printfn "All tasks finished %A"

    renderCurrent "final" ()

    0 // return an integer exit code
