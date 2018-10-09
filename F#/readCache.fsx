#r "System.Runtime.Serialization.dll"

open System.Collections.Generic
open System.IO
open System.Runtime.Serialization.Json
open System.Text

type Email = Email of string
type RawCacheItem =
    {
        email: string
        id: string
    }

let parseJson<'t> (jsonString:string)  : 't =  
    use ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(jsonString)) 
    let obj = (new DataContractJsonSerializer(typeof<'t>)).ReadObject(ms) 
    obj :?> 't


let readLines (filePath: string) = seq {
    use reader = new StreamReader (filePath)
    while not reader.EndOfStream do
        yield reader.ReadLine ()
}

let fillCache (cache: Dictionary<Email, string>) item =
    cache.Add(Email item.email, item.id)

let run () =
    let cache = new Dictionary<Email, string>()

    "cache.txt"
    |> readLines
    |> Seq.map parseJson<RawCacheItem>
    |> Seq.iter (fillCache cache)

    cache
    |> Seq.iter (printfn "%A")

run()
