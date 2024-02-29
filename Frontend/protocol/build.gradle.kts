import com.jetbrains.rd.generator.gradle.RdGenExtension
import com.jetbrains.rd.generator.gradle.RdGenTask
import org.jetbrains.kotlin.daemon.common.toHexString

plugins {
  // Version is configured in gradle.properties
  id("com.jetbrains.rdgen")
}

apply {
  plugin("com.jetbrains.rdgen")
}

val backendPluginFolderName = "Backend"
val riderBackedPluginName = "ForTea.RiderPlugin"
val backendPluginSolutionName = "ForTea.Backend.sln"

val repoRoot = projectDir.resolve("../..")
val riderBackendPluginPath = File(repoRoot, "$backendPluginFolderName/RiderPlugin/$riderBackedPluginName")
val productsHome = buildscript.sourceFile?.parentFile?.parentFile?.parentFile?.parentFile
val buildConfiguration = ext.properties["BuildConfiguration"] ?: "Debug"
val pregeneratedMonorepoPath = File(productsHome, "Plugins/_ForTea.Pregenerated")

val riderForTeaTargetsGroup = "T4"

val parentGradle = gradle.parent

fun File.writeTextIfChanged(content: String) {
  val bytes = content.toByteArray()

  if (!exists() || readBytes().toHexString() != bytes.toHexString()) {
    println("Writing $path")
    writeBytes(bytes)
  }
}

fun RdGenExtension.configureRdGen(csOutput: File, ktOutput: File, classPath: Any, suffix: String? = null) {
    verbose = true
    hashFolder = "build/rdgen"

    logger.info("Configuring rdgen params")
    sources(File(repoRoot, "Frontend/protocol/src/main/kotlin/model"))
    classpath(classPath)

    packages = "model"

    generator {
        language = "kotlin"
        transform = "asis"
        root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
        namespace = "com.jetbrains.rider.model"
        directory = "$ktOutput"
        generatedFileSuffix = suffix ?: ""
    }

    generator {
        language = "csharp"
        transform = "reversed"
        root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
        namespace = "JetBrains.Rider.Model"
        directory = "$csOutput"
        generatedFileSuffix = suffix ?: ""
    }
}

if (parentGradle == null) {
    dependencies {
        val rdLibs = rootProject.buildscript.configurations.getByName("classpath").files.filter {
            it.name.contains("rd-")
        }
        for (rdLib in rdLibs) {
            println("lib2: $rdLib")
        }

        rdGenConfiguration(files(rdLibs))
    }

    tasks.register<RdGenTask>("rdgenIndependent") {
        doFirst {
            configure<RdGenExtension> {
                val csOutput = File(riderBackendPluginPath, "Model")
                val ktOutput = File(repoRoot, "Frontend/src/main/kotlin/com/jetbrains/fortea/model")
                val riderModelJar: () -> String by rootProject.extra
                configureRdGen(csOutput, ktOutput, riderModelJar)
            }
        }
    }
} else {
    val riderModelProject = parentGradle.rootProject.project("rider-model")

    configurations.register("riderModel")
    dependencies {
        add("riderModel", riderModelProject)
    }

    tasks.register<RdGenTask>("rdgenMonorepo") {
        val riderModelConfiguration = configurations.getByName("riderModel")

        dependsOn(riderModelConfiguration)

        doFirst {
            configure<RdGenExtension> {
                val csOutput = pregeneratedMonorepoPath.resolve("BackendModel")
                val ktOutput = pregeneratedMonorepoPath.resolve("Frontend/src/com/jetbrains/fortea/model")
                val classPath = riderModelConfiguration.resolve()
                configureRdGen(csOutput, ktOutput, classPath, suffix = ".Pregenerated")
            }
        }
    }
}
