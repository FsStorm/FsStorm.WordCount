module WordCount.Topology

open FsJson
open StormDSL
open System

open WordCount.TopologyNode

let runner = Storm.autoAckBoltRunner
let emit = Storm.emit

let topology : Topology = 
    { TopologyName = "WordCount"
      Spouts = [{ Id = "spout"
                  Outputs = [ Default [ "sentence" ] ]
                  Spout = Local { Func = randomSentence Storm.simpleSpoutRunner }
                  Config = JsonNull
                  Parallelism = 5 }]
      Bolts = [{ Id = "split"
                 Inputs = [DefaultStream "spout", Shuffle]
                 Outputs = [Default ["word"]]
                 Bolt = Local { Func = splitSentence runner emit }
                 Config = JsonNull
                 Parallelism = 8 }
               { Id = "wordCount"
                 Inputs = [DefaultStream "split", Fields ["word"]]
                 Outputs = [Default ["word"; "count"]]
                 Bolt = Local { Func = wordCount runner emit }
                 Config = JsonNull
                 Parallelism = 12 }
      ]}