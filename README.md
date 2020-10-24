[![official JetBrains project](http://jb.gg/badges/official-flat-square.svg)](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub)

![ForTea Logo](https://raw.github.com/MrJul/ForTea/master/Logo/ForTea%2032x32.png "ForTea Logo") ForTea
====

About
----
ForTea is a plugin for [Rider](https://www.jetbrains.com/rider/) and [ReSharper](https://www.jetbrains.com/resharper/) which adds intelligent support for editing T4 files (`*.tt`, `*.ttinclude`, `*.t4`).
The Rider plugin also can automatically execute and preprocess T4 files.

Installation
----
#### Rider:
Starting with `Rider 2019.3`, this plugin will be installed out of the box.

#### ReSharper:
The plugin is [available](https://plugins.jetbrains.com/plugin/13469-fortea/) in JetBrains Plugin Repository and can be installed using Extension Manager from the ReSharper menu.

Features
----
 - Execution of classical T4 templates
 - Generation of C# code (aka template preprocessing)
 - Full intelligent support in T4 directives
 - Full intelligent support in C# in in-line blocks: find usages, refactorings, context actions, etc.
 - Extensive intelligent support for includes
 - Support for adding assembly and import directives through quick fixes
 - Lots of other IDE features: file structure, extend selection, refactorings, etc

License
----
Licensed under [Apache License 2.0](LICENSE).  
This plugin is a successor to [the original plugin by MrJul](https://github.com/MrJul/ForTea), to whom we are very thankful.

Building, running and debugging the plugin
----
Build instructions can be found [here](Documentation/BUILD.md).  
Run/debug instructions for Rider plugin can be found [here](Documentation/RUN_RIDER.md).  
Run/debug instructions for ReSharper plugin can be found [here](Documentation/RUN_RESHARPER.md).  

Things to know about this ReSharper plugin
----
As a ReSharper plugin, ForTea doesn't provide any Visual Studio service,
meaning there are some limitations.
Among those, syntax highlighting is fully handled by ReSharper rather than Visual Studio:
to get coloring for identifiers, you must enable _Color identifiers_ in _ReSharper Options > Code Inspection > Settings_.
Plus, there is no code outlining support yet.

Visual Basic T4 files aren't supported yet.
Custom T4 directives aren't supported yet.