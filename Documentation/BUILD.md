ForTea plugin build instructions
====
Prerequirements
----
You'll need to have [.NET Core SDK](https://dotnet.microsoft.com/download) installed.  
To build Rider plugin, you'll also need to have [JDK](https://www.oracle.com/technetwork/java/javase/downloads/index.html) installed.  
If you are a JB employee, make sure that you have an office VPN active.
Otherwise, create `Backend/Libraries` folder, build the [JetBrains.EnvDTE](https://github.com/jetbrains/jetbrains.envdte) project
and move the following files from `JetBrains.EnvDTE` build outputs into the newly created `Libraries` folder:
- `JetBrains.EnvDTE.Client.dll`
- `JetBrains.EnvDTE.Client.pdb`
- `JetBrains.EnvDTE.Host.dll`
- `JetBrains.EnvDTE.Host.pdb`

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
./build.ps1 pack
```
The build script will ask for build configuration.
You most likely want to select the 'Debug' configuration.  
The build script will also ask for a so-called wave.
It depends on the version of ReSharper you plan to install this plugin into.
It is defined as two last digits of the version before the first dot + one digit after the first dot.
For example, ReSharper 2020.2.1 would have a wave value of 202.  

Build outputs will be placed at `Backend/artifacts/Debug/`

Running/debugging the plugin
----
Run/debug instructions for Rider plugin can be found [here](RUN_RIDER.md).  
Run/debug instructions for ReSharper plugin can be found [here](RUN_RESHARPER.md).
