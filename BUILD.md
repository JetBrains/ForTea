![ForTea Logo](https://raw.github.com/MrJul/ForTea/master/Logo/ForTea%2032x32.png "ForTea Logo")

ForTea plugin build instructions
----
Building R# plugin
----
To build R# plugin, run
```
cd Backend
powershell .\build.ps1 pack
```
Build outputs will be placed at `Backend/output/Debug/`

Building Rider plugin
----
Before the first build of Rider plugin, run
```
cd Frontend
gradlew :prepare
cd ..
```
To build Rider plugin, run
```
cd Backend
msbuild
cd ..\Frontend
gradlew :buildPlugin
```
Build outputs will be placed at `Frontend/build/distributions/`
  
---
To build plugin and run an experimantal instance of rider immediately, run
```
cd Backend
msbuild
cd ..\Frontend
gradlew :runIde
```
Note: for the first run, you might need to `:buildPlugin`, then `:runIde` and manually install newly-built plugin into it. After that, plugin will be updated automatically.  
Note 2: in Rider, there is a custom tool that finds and executes T4 generator from Visual Studio. You most likely want to disable it.
