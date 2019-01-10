Validate E-mail App
===================

Simple app for validating e-mail, etc.

## App description

### Business goal
User can create an account by providing a valid e-mail address and a name.

E-mail must be checked for:
- is not empty
- is in right format (_for simplicity, it must only contains `@`_)
- is unique in our system (_for simplicity, it must not be `already@there.cz`_)
    - but this would normally need some storage check

Name must be checked for:
- is longer than 2 chars

### Process
#### Action 1 (Create Unconfirmed Account)
- User types e-mail
    - It validates an e-mail
        - if it is valid, an automatic confirmation e-mail with unique code is sent (_for simplicity, it only outputs `Email body: code for e-mail {EMAIL} is {CODE}`_)
        - if it is not valid, the error is shown to the user

#### Action 2 (Confirm Account)
- If user clicks on the link in the confirmation e-mail (_for simplicity, use only types the code from the e-mail_)
    - E-mail is activated
    - User is promted for creating an account

#### Action 3 (Activate Account)
- User will create account for activated e-mail, by setting up a name
    - Account is created

---
## Release
- _todo_

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)
- [FAKE](https://fake.build/fake-gettingstarted.html)

### Build
```bash
fake build
```

### Run action 1
```bash
dotnet src/ValidateEmailApp/bin/Release/netcoreapp2.2/ValidateEmailApp.dll action1 some@email.cz
```

### Run action 2 + 3
```bash
dotnet src/ValidateEmailApp/bin/Release/netcoreapp2.2/ValidateEmailApp.dll action2 some@email.cz e7d95cfc-0bce-44a1-8621-61047f84e766
```

**NOTE**: Use `Debug` dir instead of `Release` if you have `watch` running ...

### Watch
```bash
fake build target watch
```
