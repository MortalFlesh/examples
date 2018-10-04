// Learn more about F# at http://fsharp.org

type Person = {
    email: string
}

type Result<'TInput> =
    | Success of 'TInput
    | Failure of Error

and Error =
    | ErrorMessage of string
    | ErrorMessageWithPath of string * string

[<AutoOpen>]
[<RequireQualifiedAccess>]
module Parser =
    let parseArgs = function
        | [| input |] -> Success input
        | invalidInput -> (sprintf "Not a valid input - %A given" invalidInput, "Parser.parseArgs") |> ErrorMessageWithPath |> Failure

[<AutoOpen>]
[<RequireQualifiedAccess>]
module Validator =
    let notBlank input =
        match input |> String.length > 0 with
        | true -> Success input
        | false -> (sprintf "Input \"%s\" is blank" input) |> ErrorMessage |> Failure

    let email (input: string) =
        match input.Contains("@") with
        | true -> Success input
        | false -> (sprintf "Value \"%s\" is not a valid e-mail address" input) |> ErrorMessage |> Failure

[<AutoOpen>]
[<RequireQualifiedAccess>]
module Repository =
    let savePerson email =
        try Success { email = email }
        with _ -> "Some db error" |> ErrorMessage |> Failure

let canonicalizeEmail (email: string) =
    email.ToLower()

let output = function
    | Success person -> ("New person registered: " + person.email, 0)
    | Failure error ->
    match error with
    | ErrorMessage message -> (sprintf "Error: %s" message, 1)
    | ErrorMessageWithPath (message, path) -> (sprintf "Error: %s in %s" message path, 1)

// railway

let bind switchFunction = function
    | Success s -> switchFunction s
    | Failure f -> Failure f

let (>>=) twoTrackInput switchFunction =
    bind switchFunction twoTrackInput

let map singleTrackFunction =
    bind (singleTrackFunction >> Success)

// use-case

let registerPerson args =
    args
    >>= Parser.parseArgs
    >>= Validator.notBlank
    >>= Validator.email
    |> map canonicalizeEmail
    >>= Repository.savePerson

[<EntryPoint>]
let main argv =
    let (message, exitCode) =
        argv
        |> Success
        |> registerPerson
        |> output

    printfn "%s" message

    exitCode // return an integer exit code
