namespace AccountCreation.Domain

open System
open AccountCreation.Common

// ==================================
// This file contains the definitions of PUBLIC types (exposed at the boundary of the bounded context)
// related to the workflows
// ==================================

// ----------------------------------
// Input to the domain

type UnvalidatedEmail = UnvalidatedEmail of string

type UnvalidatedUnconfirmedAccount = {
    Email: string
    Code: string
}

type UnvalidatedName = UnvalidatedName of string

// ----------------------------------
// Output from the domain (success)

type ValidEmail = ValidEmail of EmailAddress
type ConfirmationCode = ConfirmationCode of Code

type UnconfirmedAccount = {
    Email: ValidEmail
    ConfirmationCode: ConfirmationCode
}

type ActiveEmail = ActiveEmail of ValidEmail

type ActivateAccountResponse =
    | Yes of ActiveEmail * UnvalidatedName
    | No

type ConfirmedAccount = {
    Email: ActiveEmail
}

type UserName = UserName of Name

type ActiveAccount = {
    Email: ActiveEmail
    Name: UserName
}

// ----------------------------------
// Error outputs

type ValidationError = ValidationError of string

type EmailValidationError =
    | WrongEmailAddress of EmailAddressError
    | NotUnique of ValidationError

type UnconfirmedAccountCreationFailed =
    | EmailValidationFailed of EmailValidationError
    | ConfirmationEmailNotSent

type ConfirmationError =
    | InvalidInactiveAccount
    | WrongEmailCodeCombination

type WrongAnswerError = WrongAnswerError of string

type ActivationFailed =
    | ConfirmationError of ConfirmationError
    | WrongAnswer of WrongAnswerError
    | NotWanted
    | WrongName of NameError

// ----------------------------------
// Workflows (actions)

[<RequireQualifiedAccess>]
module Action =
    type CreateUnconfirmedAccount =
        UnvalidatedEmail -> Result<UnconfirmedAccount, UnconfirmedAccountCreationFailed>

    type ConfirmAccount =
        UnvalidatedUnconfirmedAccount -> Result<ActivateAccountResponse, ActivationFailed>

    type ActivateAccount =
        ActivateAccountResponse -> Result<ActiveAccount, ActivationFailed>
