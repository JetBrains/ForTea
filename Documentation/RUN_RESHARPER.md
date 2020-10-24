ForTea *ReSharper* plugin run/debug instructions
====
Prerequirements:
----
Same as for building the plugin.  
Additionally, you'll need an instance of Visual Studio with ReSharper.  

Running ReSharper plugin
----
To run ReSharper plugin, do the following:
 - [Build](BUILD.md) the plugin;
 - Open Visual Studio;
 - Add `Backend/artifacts/Debug` or `Backend/artifacts/Release` as an extension source;
   That can be done using `Extensions > ReSharper > Extension Manager > Options > Add`;
 - Save;
 - Install the ReSharper plugin using the `Extension manager`;
 - Restart Visual Studio

Debugging ReSharper plugin
----
To debug ReSharper plugin,
build and run the plugin,
then open `Backend/ForTea.ReSharper.sln` in [Rider](https://www.jetbrains.com/rider/),
invoke `Attach to process` action and select `devenv` process.

Running/debugging Rider plugin
----
Run/debug instructions for Rider plugin can be found [here](RUN_RIDER.md).
