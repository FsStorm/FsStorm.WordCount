module WordCount.TopologyNode

open FsJson
open Storm
open System
open System.Collections.Generic

let rnd = new System.Random()
let sentences = [ "the cow jumped over the moon"
                  "an apple a day keeps the doctor away"
                  "four score and seven years ago"
                  "snow white and the seven dwarfs"
                  "i am at two with nature" ]

let randomSentence runner cfg =
    let next emit = 
        fun () ->
            async {
                System.Threading.Thread.Sleep(100)
                tuple [sentences.[rnd.Next(0,sentences.Length)]] |> emit
            }
    next |> runner

let splitSentence runner emit cfg = 
    let execute =
        fun (msg : Json) -> 
            async { 
                msg?tuple.[0].Val.ToString().Split(' ')
                |> Array.iter (fun w -> tuple [w] |> emit)
            }
    execute |> runner

let counts = Dictionary<string, int>()
let wordCount runner emit cfg = 
    let execute = 
        fun (msg : Json) -> 
            async {
                let word = msg?tuple.[0].Val
                if counts.ContainsKey(word) = false then counts.Add(word, 1)
                else counts.[word] <- counts.[word] + 1

                Storm.stormLog(String.Format("Counted '{0}' {1} times", word, counts.[word])) Storm.LogLevel.Info

                tuple [word; counts.[word].ToString()] |> emit
            }
    execute |> runner