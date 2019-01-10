namespace AccountCreation.Common

open System

// ===============================
// Simple types and constrained types related to the AcountCreation domain
//
// E.g. Single case discriminated unions (aka wrappers), enums, etc
// ===============================

type String20to50 = private String20to50 of string

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
            sprintf "%s must be longer than %i chars" fieldName maxLength |> Error
        elif string.Length > maxLength then
            sprintf "%s must be shorter than %i chars" fieldName maxLength |> Error
        else
            string
            |> constructor
            |> Ok

module String20to50 =
    let value (String20to50 string) = string

    let create fieldName string =
        ConstrainedType.createString fieldName String20to50 20 50 string
