FsStorm.WordCount
=======

A simple example of the usage of the [FsStorm][fsStorm] library. Inspired by the example in [storm-starter][stormStarter]

# Building
Build in Visual Studio.

# Submitting the topology
Have a local [Storm](https://storm.apache.org/downloads.html) installed and running.
Make sure F# interpreter is in the path and from the build output folder (/bin/Debug by default):
```
fsi Submit.fsx
```

# Seeing the topology in action
Open [Storm UI](http://localhost:8080/) and see the Storm worker logs for runtime details.

[fsStorm]:https://github.com/FsStorm/FsStorm
[stormStarter]:https://github.com/apache/storm/blob/master/examples/storm-starter/src/jvm/storm/starter/WordCountTopology.java
