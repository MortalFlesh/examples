// Learn more about F# at http://fsharp.org

open System
open System.Threading
open System.Diagnostics
open System.Collections.Concurrent
open MF.ConsoleStyle

let rand min max =
    //(new Random()).Next(min, max)
    300

let renderMessageAsync message =
    async {
        let time = rand 200 500
        do! Async.Sleep time
        Console.WriteLine (sprintf "Render %s after %ims" message time)
        return (message, sprintf "%ims" time)
    }

let renderMessageWithTimout message =
    let time = rand 200 1000
    Thread.Sleep time
    //Console.messagef2 "Render %s after %ims" message time
    (message, sprintf "%ims" time)

let renderMessage message =
    Console.messagef "- render %s" message
    (message, "")

let saveToState (state: ConcurrentDictionary<string, int>) message =
    state.AddOrUpdate (
        message,
        1,
        fun _ oldStats -> oldStats + 1
    ) |> ignore
    (message, "")

let messure something =
    let stopWatch = Stopwatch.StartNew()
    something ()
    stopWatch.Stop()
    Console.messagef "\t=== Execution lasts %f ms ===\n" stopWatch.Elapsed.TotalMilliseconds

let prefixMessage prefix message =
    sprintf "[%s] %s" prefix message

[<EntryPoint>]
let main argv =
    Console.title "Async example"
    let max = 10
    let messages = [ for i in 1 .. max do yield sprintf "message - %02i" i ]

    let first _ =
        messure (fun _ ->
            Console.sectionf "1. Run %i messages synchronously" max
            messages
            |> List.map ((prefixMessage "First") >> renderMessageWithTimout)
            |> Console.options "\nAfter computation"
        )

    let second _ =
        messure (fun _ ->
            Console.sectionf "2. Run %i messages synchonously in parallel" max
            messages
            |> List.map ((prefixMessage "Second") >> renderMessageAsync)
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Console.options "\nAfter computation"
        )

    let third _ =
        messure (fun _ ->
            Console.sectionf "3. Run %i messages making them async synchonously in parallel" max
            messages
            |> List.map ((prefixMessage "Third") >> (fun m -> async {
                return renderMessageWithTimout m
            }))
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Console.options "\nAfter computation"
        )

    //first()
    //second()
    //third()

    let runAllAsync() =
        [ first; second; third ]
        |> List.map (fun p -> async { p() })
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Console.messagef "All 3 tasks completed ... %A"

    // ========== Another ==========
    let state = new ConcurrentDictionary<string, int>()
    //state.["current"] <- 0

    let mutable currentState = 0
    let incrementHandled a =
        currentState <- currentState + 1
        state.AddOrUpdate (
            "current",
            1,
            fun _ oldState -> oldState + 1
        ) |> ignore
        a

    Console.title "Another computations ..."
    let doMagic number prefix action =
        messure (fun _ ->
            Console.sectionf2 "%i. Run %i messages" number max
            messages
            |> List.map ((prefixMessage prefix) >> action >> incrementHandled)
            |> Console.options (sprintf "\n%i. After computation" number)
        )

    let showState () =
        Console.WriteLine (
            sprintf
                "\n===================================\n= Current (%s): %02i [%02i] =\n===================================\n"
                (DateTime.Now.ToString("HH:mm:ss.fff"))
                currentState
                state.["current"]
        )

    let saveToState = saveToState state

    [
        async { doMagic 1 "First" saveToState }
        async { doMagic 2 "Second" saveToState }
        async { doMagic 3 "Third" saveToState }
        async {
            for _ in 1 .. max / 2 do
                do! Async.Sleep 100
                showState()
        }
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Console.messagef "All tasks completed ... %A"

    showState()

    Console.table ["State.Count"; "CurrentState"; "State.current"] [
        [ state.Count - 1; currentState; state.["current"] ]
        |> List.map string
    ]
    state
    |> Seq.map (fun x -> (x.Key, x.Value |> string))
    |> Seq.sortBy fst
    |> Console.options "State:"

    Console.success "Done"

    0 // return an integer exit code
