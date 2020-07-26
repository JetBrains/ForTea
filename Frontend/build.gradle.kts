import com.jetbrains.rd.generator.gradle.RdgenParams
import org.gradle.api.tasks.testing.logging.TestExceptionFormat
import org.jetbrains.grammarkit.tasks.GenerateLexer
import org.jetbrains.grammarkit.tasks.GenerateParser
import org.jetbrains.intellij.tasks.PrepareSandboxTask
import org.jetbrains.intellij.tasks.RunIdeTask
import org.jetbrains.kotlin.daemon.common.toHexString
import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

buildscript {
  repositories {
    maven { setUrl("https://cache-redirector.jetbrains.com/www.myget.org/F/rd-snapshots/maven") }
    maven { setUrl("https://cache-redirector.jetbrains.com/dl.bintray.com/kotlin/kotlin-eap") }
    mavenCentral()
  }
  dependencies {
    classpath("com.jetbrains.rd:rd-gen:0.201.57")
    classpath("org.jetbrains.kotlin:kotlin-gradle-plugin:1.3.50")
  }
}

plugins {
  id("org.jetbrains.intellij") version "0.4.13"
  id("org.jetbrains.grammarkit") version "2019.3"
  id("me.filippov.gradle.jvm.wrapper") version "0.9.3"
}

apply {
  plugin("kotlin")
  plugin("com.jetbrains.rdgen")
  plugin("org.jetbrains.grammarkit")
}

repositories {
  mavenCentral()
  maven { setUrl("https://cache-redirector.jetbrains.com/dl.bintray.com/kotlin/kotlin-eap") }
}

grammarKit {
  grammarKitRelease = "2019.3"
}

val baseVersion = "2020.3"
val buildCounter = ext.properties["build.number"] ?: "9999"
version = "$baseVersion.$buildCounter"

intellij {
  type = "RD"
  val dir = file("build/rider")
  if (dir.exists()) {
    logger.lifecycle("*** Using Rider SDK from local path " + dir.absolutePath)
    localPath = dir.absolutePath
  } else {
    logger.lifecycle("*** Using Rider SDK from intellij-snapshots repository")
    version = "$baseVersion-SNAPSHOT"
  }

  instrumentCode = false
  downloadSources = false
  updateSinceUntilBuild = false

  // Workaround for https://youtrack.jetbrains.com/issue/IDEA-179607
  setPlugins("rider-plugins-appender")
}

val backendPluginFolderName = "Backend"
val riderBackedPluginName = "ForTea.RiderPlugin"
val backendPluginSolutionName = "ForTea.Backend.sln"

val repoRoot = projectDir.parentFile!!
val backendPluginPath = File(repoRoot, backendPluginFolderName)
val riderBackendPluginPath = File(repoRoot, riderBackedPluginName)
val backendPluginSolutionPath = File(backendPluginPath, backendPluginSolutionName)
val buildConfiguration = ext.properties["BuildConfiguration"] ?: "Debug"

val pluginFiles = listOf(
  "output/ForTea.Core/$buildConfiguration/ForTea.Core",
  "output/ForTea.RiderPlugin/$buildConfiguration/ForTea.RiderPlugin",
  "output/JetBrains.TextTemplating/$buildConfiguration/JetBrains.TextTemplating"
)

val dotNetSdkPath by lazy {
  val sdkPath = intellij.ideaDependency.classes.resolve("lib").resolve("DotNetSdkForRdPlugins")
  if (sdkPath.isDirectory.not()) error("$sdkPath does not exist or not a directory")

  println("SDK path: $sdkPath")
  return@lazy sdkPath
}

val nugetConfigPath = File(repoRoot, "NuGet.Config")
val dotNetSdkPathPropsPath = File("build", "DotNetSdkPath.generated.props")

val riderForTeaTargetsGroup = "T4"

fun File.writeTextIfChanged(content: String) {
  val bytes = content.toByteArray()

  if (!exists() || readBytes().toHexString() != bytes.toHexString()) {
    println("Writing $path")
    writeBytes(bytes)
  }
}

configure<RdgenParams> {
  val csOutput = File(repoRoot, "Backend/ForTea.RiderPlugin/Protocol")
  val ktOutput = File(repoRoot, "Frontend/src/main/kotlin/com/jetbrains/fortea/protocol")

  verbose = true
  hashFolder = "build/rdgen"
  logger.info("Configuring rdgen params")
  classpath({
    logger.info("Calculating classpath for rdgen, intellij.ideaDependency is ${intellij.ideaDependency}")
    val sdkPath = intellij.ideaDependency.classes
    val rdLibDirectory = File(sdkPath, "lib/rd").canonicalFile
    "$rdLibDirectory/rider-model.jar"
  })
  sources(File(repoRoot, "Frontend/protocol/src/main/kotlin/model"))
  packages = "model"

  generator {
    language = "kotlin"
    transform = "asis"
    root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
    namespace = "com.jetbrains.rider.model"
    directory = "$ktOutput"
  }

  generator {
    language = "csharp"
    transform = "reversed"
    root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
    namespace = "JetBrains.Rider.Model"
    directory = "$csOutput"
  }
}

tasks {
  withType<RunIdeTask> {
    // IDEs from SDK are launched with 512m by default, which is not enough for Rider.
    // Rider uses this value when launched not from SDK
    maxHeapSize = "1500m"
  }

  withType<PrepareSandboxTask> {
    val files = pluginFiles.map { "$it.dll" } + pluginFiles.map { "$it.pdb" }
    val paths = files.map { File(backendPluginPath, it) }

    paths.forEach {
      from(it) {
        into("${intellij.pluginName}/dotnet")
      }
    }

    into("${intellij.pluginName}/projectTemplates") {
      from("projectTemplates")
    }

    doLast {
      paths.forEach {
        val file = file(it)
        if (!file.exists()) throw RuntimeException("File $file does not exist")
        logger.warn("$name: ${file.name} -> $destinationDir/${intellij.pluginName}/dotnet")
      }
    }
  }

  val generateT4Lexer = task<GenerateLexer>("generateT4Lexer") {
    source = "src/main/kotlin/com/jetbrains/fortea/lexer/_T4Lexer.flex"
    targetDir = "src/main/java/com/jetbrains/fortea/lexer"
    targetClass = "_T4Lexer"
    purgeOldFiles = true
  }

  task<GenerateParser>("generateT4Parser") {
    source = "src/main/kotlin/com/jetbrains/fortea/parser/T4.bnf"
    this.targetRoot = "src/main/java"
    purgeOldFiles = true
    this.pathToParser = "fakePathToParser" // I have no idea what should be inserted here, but this works
    this.pathToPsiRoot = "fakePathToPsiRoot" // same
  }

  withType<KotlinCompile> {
    kotlinOptions.jvmTarget = "1.8"
    dependsOn(generateT4Lexer)
  }

  withType<Test> {
    dependsOn(generateT4Lexer)
    useTestNG()
    environment("LOCAL_ENV_RUN", "true")
    testLogging {
      showStandardStreams = true
      exceptionFormat = TestExceptionFormat.FULL
    }
    outputs.upToDateWhen { false }
    ignoreFailures = true
  }

  create("writeDotNetSdkPathProps") {
    group = riderForTeaTargetsGroup
    doLast {
      dotNetSdkPathPropsPath.writeTextIfChanged("""<Project>
  <PropertyGroup>
    <DotNetSdkPath>$dotNetSdkPath</DotNetSdkPath>
  </PropertyGroup>
</Project>
"""
      )
    }
  }

  create("writeNuGetConfig") {
    group = riderForTeaTargetsGroup
    doLast {
      nugetConfigPath.writeTextIfChanged(
        """<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="resharper-sdk" value="$dotNetSdkPath" />
  </packageSources>
</configuration>
"""
      )
    }
  }

  getByName("assemble") {
    doLast {
      logger.lifecycle("Plugin version: $version")
      logger.lifecycle("##teamcity[buildNumber '$version']")
    }
  }

  create("prepare") {
    group = riderForTeaTargetsGroup
    dependsOn("rdgen", "writeNuGetConfig", "writeDotNetSdkPathProps")
  }

  getByName("buildSearchableOptions") {
    // A kind of hack.
    // The task is broken outside of Rider
    // and cannot be performed when building standalone plugin
    // Assumption: plugin is built in Release mode if and only if
    // it is built on the server as Ridier bundled plugin
    enabled = buildConfiguration == "Release"
  }
}

defaultTasks("prepare")

// workaround for https://youtrack.jetbrains.com/issue/RIDER-18697
dependencies {
  testCompile("xalan", "xalan", "2.7.2")
}
