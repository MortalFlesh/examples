module FromJson =
    open System.Runtime.Serialization.Json
    open System.Text

    // http://www.fssnip.net/1l/title/Convert-an-object-to-json-and-json-to-object
    let parseJson<'t> (jsonString:string)  : 't =  
        use ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(jsonString)) 
        let obj = (new DataContractJsonSerializer(typeof<'t>)).ReadObject(ms) 
        obj :?> 't

module ToJson =
    open Newtonsoft.Json

    let serialize obj =
        JsonConvert.SerializeObject obj

    let serializePretty obj =
        JsonConvert.SerializeObject(obj, Formatting.Indented)
