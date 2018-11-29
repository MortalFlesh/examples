#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
#r "netstandard"
#r "Facades/netstandard" // https://github.com/ionide/ionide-vscode-fsharp/issues/839#issuecomment-396296095
#endif

open System

open Fake.Core
open Fake.DotNet
open Fake.IO

let serverPath = Path.getFullName "./src/Server"
let deployDir = Path.getFullName "./deploy"

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

let openBrowser url =
    //https://github.com/dotnet/corefx/issues/10361
    Command.ShellCommand url
    |> CreateProcess.fromCommand
    |> CreateProcess.ensureExitCodeWithMessage "opening browser failed"
    |> Proc.run
    |> ignore

Target.create "Clean" (fun _ ->
    Shell.cleanDirs [deployDir]
)

Target.create "Build" (fun _ ->
    runDotNet "build" serverPath
)
Target.create "Run" (fun _ ->
    let server = async {
        runDotNet "watch run" serverPath
    }
    let browser = async {
        do! Async.Sleep 5000
        openBrowser "http://localhost:8085"
    }

    let tasks = [
        server
        browser
    ]

    tasks
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)
Target.create "Hello" (fun _ ->
    printfn "Hello world :)"
)

open Fake.Core.TargetOperators

"Clean"
    ==> "Build"

"Clean"
    ==> "Run"

Target.runOrDefaultWithArguments "Build"
