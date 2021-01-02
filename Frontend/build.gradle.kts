import com.jetbrains.rd.generator.gradle.RdGenExtension
import org.gradle.api.tasks.testing.logging.TestExceptionFormat
import org.jetbrains.grammarkit.tasks.GenerateLexer
import org.jetbrains.grammarkit.tasks.GenerateParser
import org.jetbrains.intellij.tasks.PrepareSandboxTask
import org.jetbrains.intellij.tasks.RunIdeTask
import org.jetbrains.kotlin.daemon.common.toHexString
import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
  id("org.jetbrains.intellij") version "0.6.4"
  id("org.jetbrains.grammarkit") version "2020.2.1"
  id("me.filippov.gradle.jvm.wrapper") version "0.9.3"
  id ("com.jetbrains.rdgen") version "0.203.161"
  kotlin("jvm") version "1.4.10"
}

apply {
  plugin("kotlin")
  plugin("com.jetbrains.rdgen")
  plugin("org.jetbrains.grammarkit")
}

repositories {
  mavenCentral()
}

val baseVersion = "2021.1"
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

configure<RdGenExtension> {
  val csOutput = File(repoRoot, "Backend/ForTea.RiderPlugin/Model")
  val ktOutput = File(repoRoot, "Frontend/src/main/kotlin/com/jetbrains/fortea/model")

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

  val generateT4Parser = task<GenerateParser>("generateT4Parser") {
    source = "src/main/kotlin/com/jetbrains/fortea/parser/T4.bnf"
    this.targetRoot = "src/main/java"
    purgeOldFiles = true
    this.pathToParser = "fakePathToParser" // I have no idea what should be inserted here, but this works
    this.pathToPsiRoot = "fakePathToPsiRoot" // same
  }

  withType<KotlinCompile> {
    kotlinOptions.jvmTarget = "1.8"
    this.kotlinOptions.freeCompilerArgs = listOf("-Xjvm-default=enable")
  }

  withType<Test> {
    useTestNG()
    environment("NO_FS_ROOTS_ACCESS_CHECK", true)
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
    dependsOn("rdgen", "writeNuGetConfig", "writeDotNetSdkPathProps", generateT4Lexer, generateT4Parser)
  }

  getByName("buildPlugin") {
    dependsOn("prepare")
  }
}

defaultTasks("prepare")
