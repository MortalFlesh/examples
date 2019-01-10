namespace AccountCreation.Common

open System

// ==================================
// Common compound types used throughout the AccountCreation domain
//
// Includes: EmailAddress, Code, Name etc.
// Plus common errors.
//
// ==================================

type EmailAddress = private EmailAddress of String20to50

type EmailAddressError =
    | Empty
    | WrongFormat of string

module EmailAddress =
    let value (EmailAddress (String20to50 email)) = email

    let create (email: string) =
        if email |> String.IsNullOrEmpty then Error Empty
        elif email.Contains '@' |> not then Error (WrongFormat "E-mail address must contains @")
        else
            email
            |> String20to50.create "E-mail"
            |> Result.map EmailAddress
            |> Result.mapError WrongFormat

type Code = private Code of string

module Code =
    let value (Code code) = code

    let create email =
        let uniqueHash = (Guid.NewGuid()).ToString()
        let email = email |> EmailAddress.value
        Code (sprintf "%s|%s" email uniqueHash)

type Name = private Name of String20to50

type NameError =
    | WrongFormat of string

module Name =
    let value (Name (String20to50 name)) = name

    let create name =
        name
        |> String20to50.create "Name"
        |> Result.map Name
        |> Result.mapError WrongFormat
