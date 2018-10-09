open System

type Date = Date of string

type Interaction =
    {
        created: Date
        id: int
        name: string
    }

let interactions = [
    {created=Date "2018-01-04"; id=4; name="four"; }
    {created=Date "2019-01-01"; id=1; name="one"; }
    {created=Date "2017-01-05"; id=5; name="five"; }
    {created=Date "2018-01-03"; id=3; name="three"; }
]

let latest =
    interactions
    |> List.max

printfn "latest: %A" latest
