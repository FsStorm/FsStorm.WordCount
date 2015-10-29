module WordCount.TopologyNode

open FsJson
open Storm
open System
open System.Collections.Generic
open System.IO
open System.Diagnostics

let randomSentence runner cfg =
    let count = ref 0L
    let rnd = new System.Random()
    let next emit = 
        fun () ->
            async {
                // Emit a random sentence from the configured list
                let sentences = cfg.Json?conf?sentences.Array
                // Tuple-ize and emit (with some reliability)
                tuple [sentences.[rnd.Next(0,sentences.Length)]]
                |> emit (Threading.Interlocked.Increment &count.contents)
            }
    next |> runner

let splitSentence runner emit cfg = 
    let execute =
        fun (msg : Json) -> 
            async { 
                // Emit each word from the sentence individually
                msg?tuple.[0].Val.Split(' ')
                |> Array.iter (fun w -> tuple [w] |> anchor msg |> emit)
            }
    execute |> runner

let writeCounts (counts:Dictionary<string,int>) dir =
    let file = sprintf "counts_%i.txt" (Process.GetCurrentProcess().Id)
    if not (Directory.Exists(dir)) then Directory.CreateDirectory(dir) |> ignore
    let formatCounts (c:Dictionary<string,int>) = 
        let concatWordCounts sortBy = String.Join("\n", c |> Seq.sortBy sortBy |> Seq.map (fun kvp -> sprintf "%s - %i" kvp.Key kvp.Value))
        sprintf "Total words: %i\nTotal count: %i\n===============\n\nSorted by word:\n===============\n%s\n\nSorted by count:\n===============\n%s"
            (c.Count) (c |> Seq.sumBy (fun kvp -> kvp.Value)) // Total words/count
            (concatWordCounts (fun kvp -> kvp.Key)) // Sorted alphabetically
            (concatWordCounts (fun kvp -> (~-) kvp.Value)) // Sorted in descending order of count
    File.WriteAllText(Path.Combine(dir, file), formatCounts counts)

let wordCount runner emit (counts:Dictionary<string,int>) cfg = 
    let execute = 
        fun (msg : Json) -> 
            async {
                // Track each word's count in the injected dictionary
                let word = msg?tuple.[0].Val
                if not (counts.ContainsKey(word)) then counts.Add(word, 1) else counts.[word] <- counts.[word] + 1
                writeCounts counts cfg.Json?conf?dir.Val // Optionally output the results to a file in the configured directory
                tuple [word; counts.[word]] |> anchor msg |> emit
            }
    execute |> runner