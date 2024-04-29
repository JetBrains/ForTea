import org.gradle.api.tasks.testing.logging.TestExceptionFormat
import org.jetbrains.intellij.tasks.InstrumentCodeTask
import org.jetbrains.intellij.tasks.PrepareSandboxTask
import org.jetbrains.intellij.tasks.RunIdeTask
import org.jetbrains.kotlin.daemon.common.toHexString
import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
  // Version is configured in gradle.properties
  id("me.filippov.gradle.jvm.wrapper")
  id("org.jetbrains.intellij")
  kotlin("jvm")
}

apply {
  plugin("kotlin")
}

repositories {
    maven("https://cache-redirector.jetbrains.com/intellij-dependencies")
    maven("https://cache-redirector.jetbrains.com/intellij-repository/releases")
    maven("https://cache-redirector.jetbrains.com/intellij-repository/snapshots")
    maven("https://cache-redirector.jetbrains.com/maven-central")
}

val buildNumber = ext.properties["build.number"]

dependencies {
  testImplementation(kotlin("test"))
}

val riderVersion: String by project
val buildCounter = buildNumber ?: "9999"
version = "$riderVersion.$buildCounter"

intellij {
  type.set("RD")
  val dir = file("build/rider")
  if (dir.exists()) {
    logger.lifecycle("*** Using Rider SDK from local path " + dir.absolutePath)
    localPath.set(dir.absolutePath)
  } else {
    logger.lifecycle("*** Using Rider SDK from intellij-snapshots repository")
    version.set("$riderVersion-SNAPSHOT")
  }

  downloadSources.set(false)
  updateSinceUntilBuild.set(false)

  // Workaround for https://youtrack.jetbrains.com/issue/IDEA-179607
  plugins.set(listOf("rider.intellij.plugin.appender"))
}

val isMonorepo = rootProject.projectDir != projectDir
val repoRoot: File = projectDir.parentFile
val backendPluginPath = repoRoot.resolve("Backend")
val backendPluginSolutionPath = backendPluginPath.resolve("ForTea.Backend.sln")
val buildConfiguration = ext.properties["BuildConfiguration"] ?: "Debug"

if (!isMonorepo) {
    sourceSets.getByName("main") {
        java {
            srcDir(repoRoot.resolve("Frontend/src/generated/java"))
        }
        kotlin {
            srcDir(repoRoot.resolve("Frontend/src/generated/kotlin"))
        }
    }
}

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


val riderModel: Configuration by configurations.creating {
    isCanBeConsumed = true
    isCanBeResolved = false
}

artifacts {
    add(riderModel.name, provider {
        val sdkRoot = tasks.setupDependencies.get().idea.get().classes
        sdkRoot.resolve("lib/rd/rider-model.jar").also {
            check(it.isFile) {
                "rider-model.jar is not found at $riderModel"
            }
        }
    }) {
        builtBy(tasks.setupDependencies)
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

  withType<KotlinCompile>().configureEach {
    kotlinOptions.jvmTarget = "17"
    dependsOn(":grammarkit:generateLexer", ":grammarkit:generateParser")
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

  register("pwc") {
    group = riderForTeaTargetsGroup
    dependsOn("rdgenMonorepo")
  }

  register("prepare") {
    group = riderForTeaTargetsGroup
    dependsOn(":protocol:rdgen", "writeNuGetConfig", "writeDotNetSdkPathProps", ":grammarkit:generateLexer", ":grammarkit:generateParser")
  }

  named("buildPlugin").configure {
    dependsOn("prepare")
  }

  named("test").configure {
    // A fix for https://github.com/JetBrains/gradle-intellij-plugin/issues/743.
    // Remove after it is publicly available
    dependsOn("prepareTestingSandbox")
  }

  wrapper {
    gradleVersion = "8.7"
    distributionUrl = "https://cache-redirector.jetbrains.com/services.gradle.org/distributions/gradle-${gradleVersion}-bin.zip"
  }
}

defaultTasks("prepare")
