open System
open System.IO

let replace (find: string) replace (string: string) =
    string.Replace(find, replace)

let toDate date =
    DateTime.Parse(date)

module IOUtils =
    let writeToFile (writer: StreamWriter) content =
        writer.WriteLine(sprintf "%s" content)

    let writeSeq (fileName: string) data =
        use writer = new StreamWriter(fileName, true)
        data
        |> Seq.iter (writeToFile writer)
        writer.Flush()

    let readLines (filePath: string) = seq {
        use reader = new StreamReader (filePath)
        while not reader.EndOfStream do
            yield reader.ReadLine ()
    }
