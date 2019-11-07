[![official JetBrains project](http://jb.gg/badges/incubator.svg)](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub)

![ForTea Logo](https://raw.github.com/MrJul/ForTea/master/Logo/ForTea%2032x32.png "ForTea Logo") ForTea
======

About
--------------
ForTea is a a plugin for [Rider](https://www.jetbrains.com/rider/) and [ReSharper](https://www.jetbrains.com/resharper/) that adds intelligent support for editing T4 files (`*.tt`, `*.ttinclude`, `*.t4`).
The Rider plugin also allows to autmatically execute and preprocess T4 files.

Installation
------------
ReSharper:
Visual Studio 2010, 2012, 2013, 2015, 2017 and 2019 are supported.
Latest stable ReSharper must be installed (older releases are still available for ReSharper 8.2, 9.x, 10.0, 2016.x, 2017.x, 2018.x).
To install ForTea, use Extension Manager from the ReSharper menu.

Rider:
To install ForTea, use _Settings > Plugins > Marketplace_.

Features
----------------
 - Execution of classical T4 templates
 - Generation of C# code (aka template preprocessing)
 - Full inetllignet support in T4 directives
 - Full intelligent support in C# in in-line blocks: find usages, refactorings, context actions, etc.
 - Extensive intelligent support for includes
 - Support for adding assembly and import directives through quick fixes
 - Lots of other IDE features: file structure, extend selection, refactorings, etc

Things to know about ReSharper plugin
--------------
As a ReSharper plugin, ForTea doesn't provide any Visual Studio service,
meaning there are some limitations.
Amongst those, syntax highlighting is fully handled by ReSharper rather than Visual Studio:
to get coloring for identifiers, you must enable _Color identifiers_ in _ReSharper Options > Code Inspection > Settings_.
Plus, there is no code outlining support yet.

Visual Basic T4 files aren't supported yet.
Custom T4 directives aren't supported yet.

License
----
Licensed under [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0)
This plugin is a successor of [the original plugin by MrJul](https://github.com/MrJul/ForTea), whom we are very thankful.

Building plugin
----
Build instruction can be found at BUILD.md
