// Learn more about F# at http://fsharp.org

open System
open System.IO
open FSharp.Data

type Person = {
    name: string
    age: int
}

type Schema = XmlProvider<"scheme/example.xml">

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"

    Path.Combine(__SOURCE_DIRECTORY__, "data.csv")
    |> File.ReadAllLines
    |> Seq.collect (fun line ->
        let parsed = line |> Schema.Parse

        parsed.Persons
        |> Seq.map (fun p ->
            {
                name = p.Name
                age =
                    match p.Age with
                    | Some age -> age
                    | _ -> 18
            }
        )
    )
    |> Seq.iter (printfn "%A")

    0 // return an integer exit code
