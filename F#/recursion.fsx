let rec factorial n =
    if n <= 1 then 1 
    else n * factorial (n - 1)

let rec factorial2 n =
    if n > 1 then n * factorial2 (n - 1)
    else 1 

let rec factorial3 = function
    | 0
    | 1 -> 1
    | n -> n * factorial3 (n - 1)
    
let factorial4 n =
    let rec fact n acc =
        match n with
        | 0 -> acc
        | _ -> fact (n - 1) (acc * n)
    fact n 1

let rec map f = function
    | [] -> []
    | x::xs -> f x::map f xs

let mapTR f l =
    let rec loop acc = function
        | [] -> List.rev acc
        | x::xs -> loop (f x::acc) xs
    loop [] l

let run() =
    printfn "Factorial!"
    let n = 5
    printfn "Factorial  for %i is %i" n (factorial n)
    printfn "Factorial2 for %i is %i" n (factorial2 n)
    printfn "Factorial3 for %i is %i" n (factorial3 n)
    printfn "Factorial4 for %i is %i" n (factorial4 n)

    printfn "%A" (map (fun x -> x + 2) [1;2;3])

run()
