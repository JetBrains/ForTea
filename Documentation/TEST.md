ForTea plugin test instructions
====
Prerequirements:
----
Same as for building the plugin.  
To run/debug backend tests, you'll also need [Rider](https://www.jetbrains.com/rider/).
To debug frontend tests, you'll also need [IntelliJ IDEA](https://www.jetbrains.com/idea/).

Testing backend
----
To run backend tests, open `ForTea.Backend` solution in Rider,
right-click `ForTea.Tests` project and select `Run Unit Tests` action.  

To debug backend tests, open `Unit Tests` tool window (Alt+8 on Windows),
select `Explorer` tab, right-click `ForTea.Tests` tree element
and select `Debug Selected Unit tests`.

Testing frontend
----
To run frontend tests from the command line,
build the Rider plugin and run `gradlew :test` in the `Frontend` folder.

To run or debug frontend tests from the IDE,
build the Rider plugin, open the `Frontend`
in [IntelliJ IDEA](https://www.jetbrains.com/idea/), open `Gradle` window, find `test` task
in `ForTea > Tasks > verification`, right-click it and select
`Run 'Frontend [test]'` or `Debug 'Frontend [test]'`, respectively.  
