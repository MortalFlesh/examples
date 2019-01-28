namespace Example

open MF.ConsoleStyle

module Types =
    type StringError =
        | TooLong of int
        | TooShort of int

    type String5to20 = private String5to20 of string
    module String5to20 =
        let value (String5to20 value) = value
        let create (string: string) =
            if string.Length > 20 then string.Length |> TooLong |> Result.Error
            elif string.Length < 5 then string.Length |> TooShort |> Result.Error
            else Ok (String5to20 string)


module Validation =
    open Types

    type ValidationError =
        | StringError of StringError

    let validate input =
        String5to20.create input
        |> Result.mapError StringError


module ErrorHandlingExample =
    open Types

    let errorHandlingExample _ =
        let askForInputAndValidate () =
            Console.ask "Give me some string (5-20)"
            |> Validation.validate

        let printResult = function
            | Ok success ->
                success
                |> Console.successf "You input \"%s\" is valid!"
            | Error error ->
                error
                |> Console.errorf "%A"

        let joinResults res1 res2 =
            [ res1; res2 ]
            |> List.map String5to20.value
            |> String.concat ", "

        while true do
            Console.title "Run example"

            Console.section "By by pattern matching"
            match askForInputAndValidate() with
            | Ok res1 ->
                match askForInputAndValidate() with
                | Ok res2 ->
                    Ok (joinResults res1 res2)
                | Error e -> Result.Error e
            | Error e -> Result.Error e
            |> printResult

            Console.section "By functions"
            askForInputAndValidate()
            |> Result.bind (fun res1 ->
                askForInputAndValidate()
                |> Result.map (fun res2 ->
                    joinResults res1 res2
                )
            )
            |> printResult

            Console.section "By operators"
            let (>>=) r f = Result.bind f r
            let (<!>) r f = Result.map f r

            askForInputAndValidate()
            >>= fun res1 ->
                askForInputAndValidate()
            <!> fun res2 ->
                joinResults res1 res2
            |> printResult

            Console.section "By computed expressions"
            result {
                let! res1 = askForInputAndValidate()
                let! res2 = askForInputAndValidate()

                return joinResults res1 res2
            }
            |> printResult

            Console.section "By list of results"
            [
                askForInputAndValidate()
                askForInputAndValidate()
            ]
            |> Result.sequence
            |> Result.map (fun results ->
                results
                |> List.map String5to20.value
                |> String.concat ", "
            )
            |> printResult

            Console.section "By list of results in computed expression"
            result {
                let! results =
                    [
                        askForInputAndValidate()
                        askForInputAndValidate()
                    ]
                    |> Result.sequence

                return
                    results
                    |> List.map String5to20.value
                    |> String.concat ", "
            }
            |> printResult
