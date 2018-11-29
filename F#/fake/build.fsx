#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open System

open Fake.Core
open Fake.DotNet
open Fake.IO

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

// Properties
let sourceDir = "."
let testDir  = "./tests"

// Targets
Target.create "Clean" (fun _ ->
    //!! "./**/bin"
    //++ "./**/obj"
    //|> Shell.cleanDirs 

    [ sourceDir; testDir ]
    |> List.collect (fun dir ->
        ["bin"; "obj"]
        |> List.map (sprintf "%s/%s" dir)
    )
    |> Shell.cleanDirs
)

Target.create "BuildApp" (fun _ ->
    runDotNet "build" sourceDir
)

Target.create "Run" (fun _ ->
    runDotNet "run" sourceDir
)

Target.create "Watch" (fun _ ->
    runDotNet "watch run" sourceDir
)

Target.create "Test" (fun _ ->
    runDotNet "run" testDir
)

// Dependencies
open Fake.Core.TargetOperators

"Clean"
    ==> "BuildApp"

"BuildApp"
    ==> "Watch"

"BuildApp"
    ==> "Test"

// start build
Target.runOrDefault "BuildApp"
