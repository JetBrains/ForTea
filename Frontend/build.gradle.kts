import com.jetbrains.plugin.structure.base.utils.isFile
import org.gradle.api.tasks.testing.logging.TestExceptionFormat
import org.jetbrains.intellij.platform.gradle.Constants
import org.jetbrains.intellij.platform.gradle.TestFrameworkType
import org.jetbrains.intellij.platform.gradle.tasks.PrepareSandboxTask
import org.jetbrains.intellij.platform.gradle.tasks.RunIdeTask
import org.jetbrains.kotlin.daemon.common.toHexString
import org.jetbrains.kotlin.gradle.tasks.KotlinCompile
import kotlin.io.path.absolute
import kotlin.io.path.absolutePathString
import kotlin.io.path.isDirectory

plugins {
  // Versions are configured in gradle.properties
  id("me.filippov.gradle.jvm.wrapper")
  id("org.jetbrains.intellij.platform")
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
    intellijPlatform {
        defaultRepositories()
        jetbrainsRuntime()
    }
}

val buildNumber = ext.properties["build.number"]

dependencies {
    testImplementation(kotlin("test"))
}

val riderBaseVersion: String by project
val buildCounter = buildNumber ?: "9999"
version = "$riderBaseVersion.$buildCounter"

dependencies {
    intellijPlatform {
        with(file("build/rider")) {
            when {
                exists() -> {
                    logger.lifecycle("*** Using Rider SDK from local path $this")
                    local(this)
                }

                else -> {
                    logger.lifecycle("*** Using Rider SDK from intellij-snapshots repository")
                    rider("$riderBaseVersion-SNAPSHOT")
                }
            }
        }

        jetbrainsRuntime()

        instrumentationTools()

        // Workaround for https://youtrack.jetbrains.com/issue/IDEA-179607
        bundledPlugin("rider.intellij.plugin.appender")

        testFramework(TestFrameworkType.Bundled)
    }
}

val isMonorepo = rootProject.projectDir != projectDir
val repoRoot: File = projectDir.parentFile
val backendPluginPath = repoRoot.resolve("Backend")
val backendPluginSolutionPath = backendPluginPath.resolve("ForTea.Backend.sln")
val buildConfiguration = ext.properties["BuildConfiguration"] ?: "Debug"

intellijPlatform {
    buildSearchableOptions = buildConfiguration == "Release"
}

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

val platformLibConfiguration: Configuration by configurations.creating {
    isCanBeConsumed = true
    isCanBeResolved = false
}

val platformLibFile = project.layout.buildDirectory.file("platform.lib.txt")
val resolvePlatformLibPath = tasks.create("resolvePlatformLibPath") {
    dependsOn(Constants.Tasks.INITIALIZE_INTELLIJ_PLATFORM_PLUGIN)
    outputs.file(platformLibFile)
    doLast {
        platformLibFile.get().asFile.writeTextIfChanged(intellijPlatform.platformPath.resolve("lib").absolutePathString())
    }
}

artifacts {
    add(riderModel.name, provider {
        intellijPlatform.platformPath.resolve("lib/rd/rider-model.jar").also {
            check(it.isFile) {
                "rider-model.jar is not found at $riderModel"
            }
        }
    }) {
      builtBy(Constants.Tasks.INITIALIZE_INTELLIJ_PLATFORM_PLUGIN)
    }

    add(platformLibConfiguration.name, provider { resolvePlatformLibPath.outputs.files.singleFile }) {
        builtBy(resolvePlatformLibPath)
    }
}

tasks {
    val dotNetSdkPath by lazy {
        val sdkPath = intellijPlatform.platformPath.resolve("lib/DotNetSdkForRdPlugins").absolute()
        if (sdkPath.isDirectory().not()) error("$sdkPath does not exist or not a directory")

        println("SDK path: $sdkPath")
        return@lazy sdkPath
    }

    val compileBackend by registering(Exec::class) {
        dependsOn("prepare")
        workingDir = backendPluginPath
        executable = "dotnet"
        args(listOf("build", backendPluginSolutionPath))
    }

  withType<RunIdeTask>().configureEach {
    // IDEs from SDK are launched with 512mb by default, which is not enough for Rider.
    // Rider uses this value when launched not from SDK
    maxHeapSize = "1500m"
  }

  withType<PrepareSandboxTask>().configureEach {
      dependsOn(compileBackend)

    val files = (pluginFiles + libraryFiles).map { "$it.dll" } + pluginFiles.map { "$it.pdb" }
    val paths = files.map { File(backendPluginPath, it) }

    paths.forEach {
      from(it) {
        into("${intellijPlatform.projectName.get()}/dotnet")
      }
    }

    into("${intellijPlatform.projectName.get()}/projectTemplates") {
      from("projectTemplates")
    }

    doLast {
      paths.forEach {
        val file = file(it)
        if (!file.exists()) throw RuntimeException("File $file does not exist")
        logger.warn("$name: ${file.name} -> $destinationDir/${intellijPlatform.projectName.get()}/dotnet")
      }
    }
  }

  withType<KotlinCompile>().configureEach {
    kotlinOptions.jvmTarget = "17"
    dependsOn(":protocol:rdgen", ":grammarkit:generateLexer", ":grammarkit:generateParser")
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

  wrapper {
    gradleVersion = "8.7"
    distributionUrl = "https://cache-redirector.jetbrains.com/services.gradle.org/distributions/gradle-${gradleVersion}-bin.zip"
  }
}

defaultTasks("prepare")
