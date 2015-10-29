module WordCount.Topology

open FsJson
open StormDSL
open System

open WordCount.TopologyNode
open System.Collections.Generic

let spoutRunner = Storm.reliableSpoutRunner Storm.defaultHousekeeper
let boltRrunner = Storm.autoAckBoltRunner
let emit = Storm.emit

let sentences = [ "the cow jumped over the moon"
                  "an apple a day keeps the doctor away"
                  "four score and seven years ago"
                  "snow white and the seven dwarfs"
                  "i am at two with nature" ]

let topology : Topology = 
    { TopologyName = "WordCount"
      Spouts = [{ Id = "randomSentence"
                  Outputs = [Default ["sentence"]]
                  Spout = Local { Func = randomSentence spoutRunner }
                  Config = jval ["sentences", jval sentences
                                 "topology.max.spout.pending", jval 1]
                  Parallelism = 1 }]
      Bolts =  [{ Id = "splitSentence"
                  Inputs = [DefaultStream "randomSentence", Shuffle]
                  Outputs = [Default ["word"]]
                  Bolt = Local { Func = splitSentence boltRrunner emit }
                  Config = JsonNull
                  Parallelism = 1 }
                { Id = "wordCount"
                  Inputs = [DefaultStream "splitSentence", Fields ["word"]]
                  Outputs = [Default ["word"; "count"]]
                  Bolt = Local { Func = wordCount boltRrunner emit (Dictionary<string,int>()) }
                  Config = jval ["dir", @"C:\temp\wordcount"]
                  Parallelism = 1 }
      ]}