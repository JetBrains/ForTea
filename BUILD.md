![ForTea Logo](https://raw.github.com/MrJul/ForTea/master/Logo/ForTea%2032x32.png "ForTea Logo") Building ForTea plugin
--------------
To build R# plugin, run
```
> cd Backend
> .\build.ps1 pack
```
or
```
$ cd Backend
$ bash ./build.sh pack
```
Build outputs will be placed at `Backend/output/Debug/`

To build Rider plugin:  
Edit `Frontend/build.gradle.kts` and replace `version = "2019.3-SNAPSHOT"` with `version = "2019.2"` (line 53)  
(That's a hack to make grammarkit-plugin work with branch 193. We are working on improving the situation)  
Run  
```
cd Frontend
gradlew :generateT4Parser
```
Edit `Frontend/build.gradle.kts` again and chage `baseVersion` back to `2019.3`.  
Run  
```
cd Frontend
gradlew :prepare
cd ..\Backend
msbuild
cd ..\Frontend
gradlew :buildPlugin
```
Build outputs will be placed at `Frontend/build/distributions/`
