ForTea plugin build instructions
====
Prerequirements
----
You'll need to have [.NET Core SDK](https://dotnet.microsoft.com/download) installed.  
To build Rider plugin, you'll also need to have [JDK](https://www.oracle.com/technetwork/java/javase/downloads/index.html) installed.

Building Rider plugin
----
To build Rider plugin, run
```bash
cd Frontend
gradlew :prepare
cd ../Backend
dotnet build ForTea.Backend.sln
cd ../Frontend
gradlew :buildPlugin
```
Build outputs will be placed at `Frontend/build/distributions/`

Building R# plugin
----
To build R# plugin, run
```bash
cd Frontend
gradlew :prepare
cd ../Backend
./build pack
```
Build outputs will be placed at `Backend/output/Debug/`