ForTea *Rider* plugin run/debug instructions
====
Prerequirements:
----
Same as for building the plugin

Running Rider plugin
----
To run Rider plugin, run
```bash
cd Frontend
gradlew :prepare
cd ../Backend
dotnet build ForTea.Backend.sln
cd ../Frontent
gradlew :runIde
```

Debugging Rider plugin
----
To debug Rider plugin frontend, run
```bash
cd Frontend
gradlew :prepare
cd ../Backend
dotnet build ForTea.Backend.sln
```
then open `Frontend` in [IntelliJ IDEA](https://www.jetbrains.com/idea/),
open 'Gradle' window, find 'runIde task',
right-click it and select 'Debug.'  

To debug Rider plugin backend,
run Rider plugin, then open `Backend/ForTea.Backend.sln` in [Rider](https://www.jetbrains.com/rider/),
invoke `Attach to process` action and select `ReSharperHost` process.

Running/debugging ReSharper plugin
----
Run/debug instructions for ReSharper plugin can be found [here](RUN_RESHARPER.md).
