ForTea *Rider* plugin run/debug instructions
====
Prerequirements:
----
Same as for building the plugin.  
To debug the plugin, you'll also need to have
[IntelliJ IDEA](https://www.jetbrains.com/idea/)
and a .NET IDE installed
([Rider](https://www.jetbrains.com/rider/) is strongly recommended as the latter).

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

Debugging Rider plugin frontend
----
To debug Rider plugin frontend, run
```bash
cd Frontend
gradlew :prepare
cd ../Backend
dotnet build ForTea.Backend.sln
```
then open `Frontend` in [IntelliJ IDEA](https://www.jetbrains.com/idea/),
open `Gradle` window, find `runIde` task in `ForTea > Tasks > intellij`,
right-click it and select `Debug Frontend [runIde]`.  

Debugging Rider plugin backend
----
To debug Rider plugin backend,
run Rider plugin, then open `Backend/ForTea.Backend.sln` in a .NET IDE
([Rider](https://www.jetbrains.com/rider/) is strongly recommended),
invoke `Attach to process` action and select `ReSharperHost` process.

Debugging Rider plugin frontend and backend simultaneously
----
Of course, they can be debugged together.  
Same as debugging Rider plugin backend,
except you need to debug the frontend instead of running it.

Running/debugging ReSharper plugin
----
Run/debug instructions for ReSharper plugin can be found [here](RUN_RESHARPER.md).
