open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let metrics() =
    [
        "# HELP Web app stats"
        "# TYPE stats_foo counter"
        "stats_foo 42"
    ]
    |> String.concat "\n"

[<EntryPoint>]
let main argv = 
    let configuration = defaultConfig

    choose [
        GET >=> choose [
            path "/"
                >=> OK "Hello from app"
            path "/metrics"
                >=> OK (metrics())
                >=> Writers.setMimeType "text/plain; version=0.0.4"
        ]
    ]
    |> startWebServer configuration

    0 // return an integer exit code
