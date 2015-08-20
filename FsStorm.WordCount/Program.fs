module WordCount.Program

open WordCount.Topology

let exePath = System.Reflection.Assembly.GetEntryAssembly().Location

[<EntryPoint>]
let main argv = 
    let logBase = if Storm.isMono() then System.Environment.GetEnvironmentVariable("HOME")+"/Logs/" else @"c:/temp/"
    Logging.log_path <- logBase + Logging.pid
    if Storm.isMono() then
        let exeName = System.IO.Path.GetFileName(exePath)
        StormProcessing.run "mono" [exeName] Topology.topology argv
    else
        StormProcessing.run exePath [] Topology.topology argv
