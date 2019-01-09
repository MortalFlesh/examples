#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

let tee f a =
    f a
    a

let sourceDir = "."

let nugetServerUrl = "http://development-nugetserver-common-stable.service.devel1-services.consul:28231"
let sources = sprintf "-s %s -s https://api.nuget.org/v3/index.json" nugetServerUrl

Target.create "Clean" (fun _ ->
    !! "**/bin"
    ++ "**/obj"
    |> Shell.cleanDirs
)

Target.create "Build" (fun _ ->
    runDotNet (sprintf "restore --no-cache %s" sources) sourceDir
    runDotNet "build --no-restore" sourceDir

    !! "**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "Watch" (fun _ ->
    runDotNet "watch run" sourceDir
)

"Clean"
    ==> "Build"
    ==> "Watch"

Target.runOrDefaultWithArguments "Build"
