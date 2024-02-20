import com.jetbrains.rd.generator.gradle.RdGenExtension
import com.jetbrains.rd.generator.gradle.RdGenTask
import org.gradle.api.tasks.testing.logging.TestExceptionFormat
import org.jetbrains.grammarkit.tasks.GenerateLexerTask
import org.jetbrains.grammarkit.tasks.GenerateParserTask
import org.jetbrains.intellij.tasks.InstrumentCodeTask
import org.jetbrains.intellij.tasks.PrepareSandboxTask
import org.jetbrains.intellij.tasks.RunIdeTask
import org.jetbrains.kotlin.daemon.common.toHexString
import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
  id("org.jetbrains.intellij") version "1.17.1"
  id("org.jetbrains.grammarkit") version "2022.3.2.1"
  id("me.filippov.gradle.jvm.wrapper") version "0.14.0"
  // Version is configured in gradle.properties
  id("com.jetbrains.rdgen")
  kotlin("jvm") version "1.9.22"
}

apply {
  plugin("kotlin")
  plugin("com.jetbrains.rdgen")
  plugin("org.jetbrains.grammarkit")
}

val buildNumber = ext.properties["build.number"]
val onTC = buildNumber != null

repositories {
  mavenCentral()

  if (onTC) {
    maven("https://cache-redirector.jetbrains.com/intellij-dependencies")
    maven("https://cache-redirector.jetbrains.com/repo1.maven.org/maven2")
  }
}

dependencies {
  testImplementation(kotlin("test"))
}

val baseVersion = "2024.1"
val buildCounter = buildNumber ?: "9999"
version = "$baseVersion.$buildCounter"

intellij {
  type.set("RD")
  val dir = file("build/rider")
  if (dir.exists()) {
    logger.lifecycle("*** Using Rider SDK from local path " + dir.absolutePath)
    localPath.set(dir.absolutePath)
  } else {
    logger.lifecycle("*** Using Rider SDK from intellij-snapshots repository")
    version.set("$baseVersion-SNAPSHOT")
  }

  downloadSources.set(false)
  updateSinceUntilBuild.set(false)

  // Workaround for https://youtrack.jetbrains.com/issue/IDEA-179607
  plugins.set(listOf("rider.intellij.plugin.appender"))
}

val backendPluginFolderName = "Backend"
val riderBackedPluginName = "ForTea.RiderPlugin"
val backendPluginSolutionName = "ForTea.Backend.sln"

val repoRoot = projectDir.parentFile!!
val backendPluginPath = File(repoRoot, backendPluginFolderName)
val riderBackendPluginPath = File(repoRoot, "$backendPluginFolderName/RiderPlugin/$riderBackedPluginName")
val backendPluginSolutionPath = File(backendPluginPath, backendPluginSolutionName)
val productsHome = buildscript.sourceFile?.parentFile?.parentFile?.parentFile?.parentFile
val buildConfiguration = ext.properties["BuildConfiguration"] ?: "Debug"
val pregeneratedMonorepoPath = File(productsHome, "Plugins/_ForTea.Pregenerated")

val pluginFiles = listOf(
  "output/ForTea.Core/$buildConfiguration/ForTea.Core",
  "output/ForTea.RiderPlugin/$buildConfiguration/ForTea.RiderPlugin",
  "output/JetBrains.TextTemplating/$buildConfiguration/JetBrains.TextTemplating"
)

// We don't need to pack EnvDTE interface assemblies, because they're already referenced in ReSharperHost
val libraryFiles = listOf(
  "output/JetBrains.TextTemplating/$buildConfiguration/JetBrains.EnvDTE.Client",
  "output/ForTea.RiderPlugin/$buildConfiguration/JetBrains.EnvDTE.Host"
)

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

tasks {
  val dotNetSdkPath by lazy {
    val sdkPath = setupDependencies.orNull?.idea?.orNull?.classes?.resolve("lib")?.resolve("DotNetSdkForRdPlugins")
      ?: error("intellij.ideaDependency.classes is null")
    if (sdkPath.isDirectory.not()) error("$sdkPath does not exist or not a directory")

    println("SDK path: $sdkPath")
    return@lazy sdkPath
  }

  withType<GenerateParserTask> {
    classpath(setupDependencies.flatMap { it.idea.map { idea -> idea.classes.resolve("lib/opentelemetry.jar") } })
  }

  withType<InstrumentCodeTask>().configureEach {
    val bundledMavenArtifacts = file("build/maven-artifacts")
    if (bundledMavenArtifacts.exists()) {
      logger.lifecycle("Use ant compiler artifacts from local folder: $bundledMavenArtifacts")
      compilerClassPathFromMaven.set(
        bundledMavenArtifacts.walkTopDown()
          .filter { it.extension == "jar" && !it.name.endsWith("-sources.jar") }
          .toList()
        + File("${setupDependencies.get().idea.get().classes}/lib/3rd-party-rt.jar")
        + File("${setupDependencies.get().idea.get().classes}/lib/util.jar")
        + File("${setupDependencies.get().idea.get().classes}/lib/util-8.jar")
      )
    } else {
      logger.lifecycle("Use ant compiler artifacts from maven")
    }
  }

  withType<RunIdeTask>().configureEach {
    // IDEs from SDK are launched with 512mb by default, which is not enough for Rider.
    // Rider uses this value when launched not from SDK
    maxHeapSize = "1500m"
  }

  withType<PrepareSandboxTask>().configureEach {
    val files = (pluginFiles + libraryFiles).map { "$it.dll" } + pluginFiles.map { "$it.pdb" }
    val paths = files.map { File(backendPluginPath, it) }

    paths.forEach {
      from(it) {
        into("${intellij.pluginName.get()}/dotnet")
      }
    }

    into("${intellij.pluginName.get()}/projectTemplates") {
      from("projectTemplates")
    }

    doLast {
      paths.forEach {
        val file = file(it)
        if (!file.exists()) throw RuntimeException("File $file does not exist")
        logger.warn("$name: ${file.name} -> $destinationDir/${intellij.pluginName.get()}/dotnet")
      }
    }
  }

  val lexerSource = "src/main/kotlin/com/jetbrains/fortea/lexer/_T4Lexer.flex"
  val parserSource = "src/main/kotlin/com/jetbrains/fortea/parser/T4.bnf"

  generateLexer.configure {
    sourceFile.set(file(lexerSource))
    targetDir.set("src/main/java/com/jetbrains/fortea/lexer")
    targetClass.set("_T4Lexer")
    purgeOldFiles.set(true)
  }

  val generateT4Parser = task<GenerateParserTask>("generateT4Parser") {
    sourceFile.set(file(parserSource))
    this.targetRoot.set("src/main/java")
    purgeOldFiles.set(true)
    this.pathToParser.set("fakePathToParser") // I have no idea what should be inserted here, but this works
    this.pathToPsiRoot.set("fakePathToPsiRoot") // same
  }

  val generateT4LexerMonorepo = task<GenerateLexerTask>("generateT4LexerMonorepo") {
    sourceFile.set(file(lexerSource))
    targetDir.set( pregeneratedMonorepoPath.resolve("Frontend/src/com/jetbrains/fortea/lexer").canonicalPath)
    purgeOldFiles.set(true)
    targetClass.set("_T4Lexer")
    purgeOldFiles.set(true)
  }

  val generateT4ParserMonorepo = task<GenerateParserTask>("generateT4ParserMonorepo") {
    sourceFile.set(file(parserSource))
    this.targetRoot.set(pregeneratedMonorepoPath.resolve("Frontend/src/").canonicalPath)
    purgeOldFiles.set(true)
    this.pathToParser.set("fakePathToParser") // I have no idea what should be inserted here, but this works
    this.pathToPsiRoot.set("fakePathToPsiRoot") // same
  }

  withType<KotlinCompile>().configureEach {
    kotlinOptions.jvmTarget = "17"
    dependsOn(generateLexer, generateT4Parser)
  }

  withType<Test>().configureEach {
    useTestNG()

    environment("NO_FS_ROOTS_ACCESS_CHECK", true)
    testLogging {
      showStandardStreams = true
      exceptionFormat = TestExceptionFormat.FULL
    }
    outputs.upToDateWhen { false }
    ignoreFailures = true
  }

  register("writeDotNetSdkPathProps") {
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

  register("writeNuGetConfig") {
    group = riderForTeaTargetsGroup
    doLast {
      nugetConfigPath.writeTextIfChanged(
        """<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="resharper-sdk" value="$dotNetSdkPath" />
    <!-- Support for open-source developers: need this to let them have EnvDTE.Client and EnvDTE.Host without access to private nuget feed-->
    <!-- <add key="local" value="${File(repoRoot, "Backend/Libraries")}" /> -->
  </packageSources>
</configuration>
"""
      )
    }
  }

  named("assemble").configure {
    doLast {
      logger.lifecycle("Plugin version: $version")
      logger.lifecycle("##teamcity[buildNumber '$version']")
    }
  }

  register<RdGenTask>("rdgenMonorepo") {
    val riderModelClassPathFile: String by project
    val classPathLines = File(riderModelClassPathFile).readLines()

    doFirst {
      configure<RdGenExtension> {
        val csOutput = pregeneratedMonorepoPath.resolve("BackendModel")
        val ktOutput = pregeneratedMonorepoPath.resolve("Frontend/src/com/jetbrains/fortea/model")

        verbose = true
        hashFolder = "build/rdgen"
        logger.info("Configuring rdgen params")
        sources(File(repoRoot, "Frontend/protocol/src/main/kotlin/model"))
        classpath(classPathLines)

        packages = "model"
        generator {
          language = "csharp"
          transform = "reversed"
          root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
          namespace = "JetBrains.Rider.Model"
          directory = "$csOutput"
          generatedFileSuffix = ".Pregenerated"
        }

        generator {
          language = "kotlin"
          transform = "asis"
          root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
          namespace = "com.jetbrains.rider.model"
          directory = "$ktOutput"
          generatedFileSuffix = ".Pregenerated"
        }
      }
    }
  }

  register<RdGenTask>("rdgenIndependent") {
    doFirst {
      configure<RdGenExtension> {
        val csOutput = File(riderBackendPluginPath, "Model")
        val ktOutput = File(repoRoot, "Frontend/src/main/kotlin/com/jetbrains/fortea/model")

        verbose = true
        hashFolder = "build/rdgen"
        logger.info("Configuring rdgen params")
        classpath({
                    logger.info("Calculating classpath for rdgen, intellij.ideaDependency is ${setupDependencies.orNull?.idea?.orNull}")
                    val sdkPath = setupDependencies.orNull?.idea?.orNull?.classes ?: error("intellij.ideaDependency.classes is null")
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
    }
  }

  register("pwc") {
    group = riderForTeaTargetsGroup
    dependsOn("rdgenMonorepo")
  }

  register("prepare") {
    group = riderForTeaTargetsGroup
    dependsOn("rdgenIndependent", "writeNuGetConfig", "writeDotNetSdkPathProps", generateLexer, generateT4Parser)
  }

  register("prepareMonorepo") {
    group = riderForTeaTargetsGroup
    dependsOn("rdgenMonorepo", generateT4LexerMonorepo, generateT4ParserMonorepo)
  }

  named("buildPlugin").configure {
    dependsOn("prepare")
  }

  named("test").configure {
    // A fix for https://github.com/JetBrains/gradle-intellij-plugin/issues/743.
    // Remove after it is publicly available
    dependsOn("prepareTestingSandbox")
  }
}

defaultTasks("prepare")
