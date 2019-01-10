namespace AccountCreation

[<RequireQualifiedAccessAttribute>]
module Action1 =
    open System
    open Types

    let execute: Action1 =
        fun validateEmail sendConfirmationEmail userInput ->
            userInput
            |> sprintf "Email was successfully send to \"%s\" address."
            |> Ok
