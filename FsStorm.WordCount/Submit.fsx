#load "StormSubmit.fsx"

let binDir = System.Environment.CurrentDirectory

//StormSubmit.makeJar binDir

StormSubmit.runTopology binDir "localhost" StormSubmit.default_nimbus_port
