// Learn more about F# at http://fsharp.org

open System
open System.Diagnostics

type Item = {
    Label: string
    Number: int
}

[<EntryPoint>]
let main argv =
    let sw = Stopwatch.StartNew()

    let total = 10000000
    printfn "For %i" total

    let transform n =
        if n % 7 = 0 then
            sprintf "Item [%i]" n
        else
            sprintf "Item [%i] | %i" n n

    let parse (i: string) =
        match i.Split("|") with
        | [| label; number |] -> { Label = label; Number = number |> int } |> Some
        | _ -> None

    let fromLength n (string: string) =
        string.Length > n

    // list
    //[0..total]                                                  // 745
    //|> List.map transform                                       // 7313
    //|> List.filter (fromLength 11)                              // 1174
    //|> List.map parse                                           // 8653
    //|> List.choose id                                           // 1452
    //|> List.fold (fun t { Number = n } -> (t + n) / total) 0    // 243
    //|> printfn "Result: %A"

    // seq
    //seq { 0..total }
    //|> Seq.map transform
    //|> Seq.filter (fromLength 11)
    //|> Seq.map parse
    //|> Seq.choose id
    //|> Seq.fold (fun t { Number = n } -> (t + n) / total) 0
    //|> printfn "Result: %A"

    // for
    //let mutable result = 0
    //for i in 0..total do
    //    let t = transform i
    //    if fromLength 11 t then
    //        let p = parse t
    //        if p.IsSome then
    //            let n = p.Value.Number
    //            result <- (result + n) / total
    //printfn "Result: %A" result

    // list - separated
    //let x = [0..total]
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> List.map (transform |> makeAsync) |> asyncMap
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> List.filter (fromLength 11)
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> List.map parse
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> List.choose id
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> List.fold (fun t { Number = n } -> (t + n) / total) 0
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //x |> printfn "Result: %A"

    // seq - separated
    //let x = seq {0..total}
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> Seq.map transform
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> Seq.filter (fromLength 11)
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> Seq.map parse
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> Seq.choose id
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //let x = x |> Seq.fold (fun t { Number = n } -> (t + n) / total) 0
    //printfn "Time: %d ms" sw.ElapsedMilliseconds; sw.Restart()
    //x |> printfn "Result: %A"

    sw.Stop()
    printfn "Lasts: %d ms" sw.ElapsedMilliseconds

    0 // return an integer exit code
