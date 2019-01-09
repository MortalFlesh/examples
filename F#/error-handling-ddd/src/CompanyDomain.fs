namespace CompanyDomain

type UserInput = {
    Mathematician: string
    Divider: string
    Divisior: string
}

type DivideIntegersAsAService = UserInput -> int

module DivideInts =
    type DivideIntegers = int -> int -> int

    let private divideInts: DivideIntegers =
        fun divider divisor ->
            divider / divisor

    let byUserInput log : DivideIntegersAsAService =
        fun { Mathematician = mathematician; Divider = divider; Divisior = divisor } ->
            log (sprintf "I'm counting %s / %s for %s" divider divisor mathematician)

            divideInts (int divider) (int divisor)
