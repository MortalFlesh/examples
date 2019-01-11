namespace AccountCreation.Common

open System

// ===============================
// Simple types and constrained types related to the AcountCreation domain
//
// E.g. Single case discriminated unions (aka wrappers), enums, etc
// ===============================

type EmailBody = EmailBody of string

type String5to50 = private String5to50 of string
type String10to50 = private String10to50 of string

// ===============================
// Reusable constructors and getters for constrained types
// ===============================

/// Useful functions for constrained types
module private ConstrainedType =

    /// Create a constrained string using the constructor provided
    /// Return Error if input is null, empty, or length > maxLen
    let createString fieldName constructor minLength maxLength string =
        if String.IsNullOrEmpty(string) then
            sprintf "%s must not be empty" fieldName |> Error
        elif string.Length < minLength then
            sprintf "%s must be longer than %i chars" fieldName minLength |> Error
        elif string.Length > maxLength then
            sprintf "%s must be shorter than %i chars" fieldName maxLength |> Error
        else
            string
            |> constructor
            |> Ok

module String5to50 =
    let value (String5to50 string) = string

    let create fieldName string =
        ConstrainedType.createString fieldName String5to50 5 50 string

module String10to50 =
    let value (String10to50 string) = string

    let create fieldName string =
        ConstrainedType.createString fieldName String10to50 10 50 string
