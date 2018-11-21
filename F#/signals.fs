open System

let main() =
    let mutable batches = 0
    let mutable shouldRun = true

    Console.CancelKeyPress.AddHandler (new ConsoleCancelEventHandler(fun sender ->
        fun e ->
            printfn "Canceled"
            e.Cancel <- true    // this cancels the process cancelation by ctrl+c
            shouldRun <- false
    ))

    while shouldRun do
        batches <- batches+1
        Threading.Thread.Sleep( 500 )

        for i in 0 .. 20 do
            Threading.Thread.Sleep( 100 )
            printfn "Batch %d - %d" batches i
    