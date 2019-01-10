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

type UnvalidatedInactiveAccount = {
    Email: string
    Code: string
}

type ActivateAccountResponse =
    | Yes of UnvalidatedName
    | No

and UnvalidatedName = UnvalidatedName of string

// ----------------------------------
// Output from the domain (success)

type ValidEmail = ValidEmail of EmailAddress
type ConfirmationCode = ConfirmationCode of Code

type UnconfirmedAccount = {
    Email: ValidEmail
    ConfirmationCode: ConfirmationCode
}

type ActiveEmail = ActiveEmail of ValidEmail

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

type ActivationFailed =
    | NotWanted
    | WrongName of NameError

// ----------------------------------
// Workflows (actions)

type CreateUnconfirmedAccount =
    UnvalidatedEmail -> Result<UnconfirmedAccount, UnconfirmedAccountCreationFailed>

type ConfirmAccount =
    UnvalidatedInactiveAccount -> Result<ConfirmedAccount, ConfirmationError>

type AskUserToActiveAccount =
    ConfirmedAccount -> ActivateAccountResponse

type ActivateAccount =
    ActivateAccountResponse -> Result<ActiveAccount, ActivationFailed>
