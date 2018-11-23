module Example.MonadicAdd

open System

// ========= logic =========
let add x y = x + y

// ========= helpers =========
type Int32 with
    static member ParseAsOption (str: string) =
        match str |> Int32.TryParse with
        | true, number -> Some number
        | _ -> None

let askFor question =
    Console.Write (sprintf "%s: " question)
    Console.ReadLine() |> Int32.ParseAsOption

let success (x, y, result) =
    printfn "[success] %i + %i = %i" x y result

let error () =
    printfn "[error] X or Y was not a number"

let renderResult = function
    | Some result -> result |> success
    | _ -> error()

// ========= compute by standard aproach =========

let computeStandardly () =
    let x = askFor "X"
    let y = askFor "Y"

    let result =
        match x, y with
        | Some x, Some y ->
            (x, y, add x y)
            |> Some
        | _ -> None

    result |> renderResult

// ========= compute by aplicative =========

let (>>=) m f = Option.bind f m

let computeAplicatively () =
    let result =
        askFor "X" >>= (fun x ->
        askFor "Y" >>= (fun y ->
            (x, y, add x y)
            |> Some
        ))

    result |> renderResult

// ========= compute by monadic computed expressions =========

type MaybeBuilder() =
    member __.Bind (m, f) = Option.bind f m
    member __.Return x = Some x

let maybe = MaybeBuilder()

let coomputeByComputedExpressions () =
    maybe {
        let! x = askFor "X"
        let! y = askFor "Y"
        return (x, y, add x y)
    }
    |> renderResult

// ========= running =========

// standard:
// X: 1
// Y: 2
// [success] 1 + 2 = 3

// aplicative:
// X: 3
// Y: 4
// [success] 3 + 4 = 7

// monads:
// X: 5
// Y: 6
// [success] 5 + 6 = 11

// ---------------------------

// standard:
// X: a
// Y: b
// [error] X or Y was not a number

// aplicative:
// X: c
// [error] X or Y was not a number

// monads:
// X: d
// [error] X or Y was not a number
