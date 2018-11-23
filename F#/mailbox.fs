module Example.Mailbox

open System

type Message =
    | Add of int
    | Fetch of AsyncReplyChannel<int>

type Queue() =
    let innerQueue =
        let (+>) x list =
            printfn "[QUEUE] Adding %i to queue" x
            x :: list

        MailboxProcessor.Start(fun inbox ->
            let rec loop list =
                async {
                    printfn "[QUEUE] Current number of items: %i" (list |> List.length)

                    match list with
                    | [] ->
                        let! message =
                            inbox.Scan(function
                                | Add _ as m -> async.Return m |> Some
                                | _ -> None
                            )
                        match message with
                        | Add x -> return! x +> list |> loop
                        | _ -> return! list |> loop
                    | _ ->
                        let! message = inbox.Receive()
                        match message with
                        | Add x -> return! x +> list |> loop
                        | Fetch(reply) ->
                            let x = list.Head
                            printfn "[QUEUE] Fetching %i from queue" x

                            reply.Reply(x)
                            return! list.Tail |> loop
                }
            loop []                 
        )

    member this.Add x = Add x |> innerQueue.Post
    member this.Fetch() = Fetch |> innerQueue.PostAndReply

let queue = Queue()

let producer id =
    async {
        let rand = Random()
        while true do
            let x = rand.Next(100)
            x |> queue.Add
            printfn "[PRODUCER][%s] produced %i" id x

            do! Async.Sleep(100 * x)
    }

let consumer id =
    async {
        while true do
            let x = queue.Fetch()
            printfn "[CONSUMER][%s] consumed %i" id x

            do! Async.Sleep(100)
    }

let run () =
    producer "first" |> Async.Start
    producer "second" |> Async.Start

    consumer "first" |> Async.Start
    consumer "second" |> Async.Start

    Console.Read() |> ignore
    0
