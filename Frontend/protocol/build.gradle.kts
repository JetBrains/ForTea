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
  // Version is configured in gradle.properties
  id("com.jetbrains.rdgen")
}

apply {
  plugin("com.jetbrains.rdgen")
}

val backendPluginFolderName = "Backend"
val riderBackedPluginName = "ForTea.RiderPlugin"
val backendPluginSolutionName = "ForTea.Backend.sln"

val repoRoot = projectDir.parentFile!!
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

val rdLibDirectory: () -> String by extra

tasks {
  if (parentGradle == null) {
    register<RdGenTask>("rdgenIndependent") {
      doFirst {
        configure<RdGenExtension> {
          val csOutput = File(riderBackendPluginPath, "Model")
          val ktOutput = File(repoRoot, "Frontend/src/main/kotlin/com/jetbrains/fortea/model")

          verbose = true
          hashFolder = "build/rdgen"
          logger.info("Configuring rdgen params")
          classpath(rdLibDirectory)
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
  }
}

if (parentGradle != null) {
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

                verbose = true
                hashFolder = "build/rdgen"
                logger.info("Configuring rdgen params")
                sources(File(repoRoot, "Frontend/protocol/src/main/kotlin/model"))
                classpath(riderModelConfiguration.resolve())

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
}
