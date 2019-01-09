DDD Error Handling
==================

Simple app for demonstrating how should be "_Errors_" handled in DDD application.

## App description

### Business goal
Let's have a company, which divide integers as a service.

You must say, who you are and then provide `Divider` and `Divisor`, service will provide a result.


### Technically
- `Watch` will start the application and it will run till the user stops it.
- For now, there is `CompanyDomain` with business type of the _Divide integers as a service_ `DivideIntegersAsAService` and its implementation
- Implementation of this type - function `byUserInput` is the only public function of our `CompanyDomain`, so the `DivideIntegersAsAService` should expose all relevant information

### Current goal
- Currently, the implementation is not yet done, so it will end up with `Not implemented yet.` exception, which you should fix!
- HINT: there is `DivideIntegers` private type of our domain, which you can implement to make it work

---
## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)
- [FAKE](https://fake.build/fake-gettingstarted.html)

### Build
```bash
fake build
```

### Run with Watch
```bash
fake build target watch
```
