import com.jetbrains.rd.generator.gradle.RdGenExtension
import com.jetbrains.rd.generator.gradle.RdGenTask
import org.jetbrains.kotlin.daemon.common.toHexString

plugins {
  // Version is configured in gradle.properties
  id("com.jetbrains.rdgen")
  id("org.jetbrains.kotlin.jvm")
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

val isMonorepo = rootProject.projectDir != projectDir.parentFile

sourceSets {
    main {
        kotlin {
            srcDir(File(repoRoot, "Frontend/protocol/src/main/kotlin/model"))
        }
    }
}

fun File.writeTextIfChanged(content: String) {
  val bytes = content.toByteArray()

  if (!exists() || readBytes().toHexString() != bytes.toHexString()) {
    println("Writing $path")
    writeBytes(bytes)
  }
}

fun RdGenExtension.configureRdGen(csOutput: File, ktOutput: File, suffix: String? = null) {
    verbose = true

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

if (isMonorepo) {
    dependencies {
        implementation(project(":rider-model"))
    }

    tasks.register<RdGenTask>("rdgenMonorepo") {
        dependsOn(sourceSets["main"].runtimeClasspath)
        classpath(sourceSets["main"].runtimeClasspath)

        configure<RdGenExtension> {
            val csOutput = pregeneratedMonorepoPath.resolve("BackendModel")
            val ktOutput = pregeneratedMonorepoPath.resolve("Frontend/src/com/jetbrains/fortea/model")
            configureRdGen(csOutput, ktOutput, suffix = ".Pregenerated")
        }
    }
} else {
    val rdVersion: String by project
    val rdKotlinVersion: String by project

    dependencies {
        implementation("com.jetbrains.rd:rd-gen:$rdVersion")
        implementation("org.jetbrains.kotlin:kotlin-stdlib:$rdKotlinVersion")
        implementation(project(mapOf(
            "path" to ":",
            "configuration" to "riderModel")))
    }

    tasks.register<RdGenTask>("rdgenIndependent") {
        classpath(sourceSets["main"].runtimeClasspath)
        dependsOn(sourceSets["main"].runtimeClasspath)

        configure<RdGenExtension> {
            val csOutput = File(riderBackendPluginPath, "Model")
            val ktOutput = File(repoRoot, "Frontend/src/main/kotlin/com/jetbrains/fortea/model")
            configureRdGen(csOutput, ktOutput)
        }
    }
}
