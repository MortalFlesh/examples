module Foo.Test

open System
open System.IO
open Expecto
open Foo

[<Tests>]
let environmentTests =
    testList "Foo" [
        testCase "should bar" <| fun _ ->
            let expected = "foo | bar: bar"
            let actual = foo "bar"
            Expect.equal expected actual ""
    ]
