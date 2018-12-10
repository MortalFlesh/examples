// Learn more about F# at http://fsharp.org

open System
open System.Threading
open System.Diagnostics
open MF.ConsoleStyle

let rand min max =
    (new Random()).Next(min, max)
    //300

let renderMessageAsync message =
    async {
        let time = rand 200 1000
        do! Async.Sleep time
        Console.WriteLine (sprintf "Render %s after %ims" message time)
        return (message, time |> string)
    }

let renderMessage message =
    let time = rand 200 1000
    Thread.Sleep time
    Console.messagef2 "Render %s after %ims" message time
    (message, time |> string)

let messure something =
    let stopWatch = Stopwatch.StartNew()
    something ()
    stopWatch.Stop()
    Console.messagef "\t=== Execution lasts %f ms ===\n" stopWatch.Elapsed.TotalMilliseconds

[<EntryPoint>]
let main argv =
    Console.title "Async example"
    let max = 10
    let messages = [ for i in 1 .. max do yield sprintf "message - %i" i ]

    messure (fun _ ->
        Console.sectionf "Run %i messages synchronously" max
        messages
        |> List.map renderMessage
        |> Console.options "\nAfter computation"
    )

    messure (fun _ ->
        Console.sectionf "Run %i messages synchonously in parallel" max
        messages
        |> List.map renderMessageAsync
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Console.options "\nAfter computation"
    )

    messure (fun _ ->
        Console.sectionf "Run %i messages making them async synchonously in parallel" max
        messages
        |> List.map (fun m -> async {
            return renderMessage m
        })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Console.options "\nAfter computation"
    )

    0 // return an integer exit code
