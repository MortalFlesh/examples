module Workshop

//
// ========= Imutabilita =========
//

// example 1

let doSomething data =
    printfn "%A" data

let example() =
    let data = [1; 2; 3]

    doSomething data

    data


// example 2

type Person = {
    name: string
}

let example2() =
    let data = { name = "Jon Snow"}

    doSomething data

    data

//
// ========= Rekurze =========
//

let rec quickSort = function
   | [] -> []
   | [oneItemOnly] -> [oneItemOnly]
   | first::rest ->
        let smaller,larger = List.partition ((>=) first) rest
        List.concat [quickSort smaller; [first]; quickSort larger]

//
// ========= Kompozice =========
//

// helpery

let replace (pattern: string) (replacement: string) (value: string) =
    value.Replace(pattern, replacement)

let toUpper (value: string) =
    value.ToUpper()

// example

let fillNameTemplate firstName surname template =
    let result = replace "{{firstName}}" firstName template
    let result = replace "{{surname}}" surname result
    let upper = toUpper result
    upper

let fillNameTemplate2 firstName surname template =
    template
    |> replace "{{firstName}}" firstName
    |> replace "{{surname}}" surname
    |> toUpper

let fillNameTemplate3 firstName surname =
    replace "{{firstName}}" firstName
    >> replace "{{surname}}" surname
    >> toUpper

// run example

let template = "Ahoj {{firstName}} {{surname}}, jak se máš?"

printfn "%s" (template |> fillNameTemplate "Jon" "Snow")

template
|> fillNameTemplate2 "Jon" "Snow"
|> printfn "%s"

template
|> fillNameTemplate3 "Jon" "Snow"
|> printfn "%s"
