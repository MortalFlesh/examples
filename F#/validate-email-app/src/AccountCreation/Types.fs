namespace AccountCreation

module Types =
    // First, what do we know...
    //
    // For action 1
    // ------------
    // User will give as some input
    //    -> UserInput (string)
    //
    // Input is validated, if it is an e-mail
    //    -> Email is
    //         unvalidated - just a user input
    //         or validated - if it passess all validation rules
    //
    // Validation gets a user input and validates it, which may ends up with different result of valid e-mail or some error
    //    -> UserInput -> Validation -> Result with a valid e-mail or an error
    //    we are validating e-mail for:
    //          - not empty
    //          - right Format
    //          - is unique
    //      -> possible errors are:
    //          -> given email is empty
    //          -> given email is not in correct format
    //          -> given email is not unique
    //          -> ... any other?
    //
    // When we have a valid e-mail, we will send a confirmation e-mail with generated code
    //      -> Code - some generated string, but must be somehow associated with the e-mail (normally saved in some storage)
    //      -> Confirmation E-mail with code in it
    //          - ... could that end up with some error?

    //
    // For action 2
    // ------------
    // User clicks on link in Confirmation e-mail, which triggers an action 2. Link contains both e-mail and generated code
    //
    // First we need to check, whether pair of e-mail and code is correct
    //      -> so it could end up with error
    //      -> ... or more errors?
    //
    // If e-mail and code are correct, we need to activate an e-mail address
    //      -> Email is
    //          unvalidated or validated
    //          or activated
    //
    // User is now asked if he wants to create an account
    // If he does not want an account, our job here is done and we just show user the result
    // If he wants to create an account, we would continue to action 3

    //
    // For action 3
    // ------------
    // He is asked for a name. And we know, that name must be longer than 2 letters. So we need to validate it ...
    //      -> UserInput -> Validation -> Result of a valid name or an arror
    //      possible errors might be:
    //          -> to short name error
    //          - but what if it is too long? we should ask a domain expert, what to do.. -> "longer name than 20 chars does not make any sense"
    //          -> to long name error
    //
    // We have an activated e-mail and a valid Name, thats all to create an account, so we do and show the result to the user
    //

    // ... now lets look at our previous implementation, how does it reflect our business domain and ubiquitous language

    type Account = {
        Email: string
        Name: string
    }
