open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared


let publicPath = Path.GetFullPath "public"
let port = 8085us

let getInitCounter() : Task<Counter> = task { return { Value = 42 } }

let mutable currentValue = 0

let current() =
    sprintf "Current: %i" currentValue

let start() =
    async {
        for _ in 0 .. 20 do
            do! Async.Sleep 1000

            currentValue <- currentValue + 1
            do printfn "Current value: %i" currentValue
    }
    |> Async.Start

    "Started ..."


let webApp = router {
    get "/" (fun next ctx ->
        task {
            return! Successful.OK (current()) next ctx
        })
    
    get "/start" (fun next ctx ->
        task {
            return! Successful.OK (start()) next ctx
        })
}

let configureSerialization (services:IServiceCollection) =
    services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let app = application {
    url ("http://127.0.0.1:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    service_config configureSerialization
    use_gzip
}

run app
