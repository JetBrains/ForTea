import com.jetbrains.rd.generator.gradle.RdGenTask

plugins {
    // Version is configured in gradle.properties
    id("com.jetbrains.rdgen")
    id("org.jetbrains.kotlin.jvm")
}

repositories {
    maven("https://cache-redirector.jetbrains.com/intellij-dependencies")
    maven("https://cache-redirector.jetbrains.com/maven-central")
}

val isMonorepo = rootProject.projectDir != projectDir.parentFile
val forTeaRepoRoot: File = projectDir.parentFile.parentFile

sourceSets {
    main {
        kotlin {
            srcDir(File(forTeaRepoRoot, "Frontend/protocol/src/main/kotlin/model"))
        }
    }
}

data class ForTeaGeneratorSettings(val csOutput: File, val ktOutput: File, val suffix: String)

val generatorOutputSettings = if (isMonorepo) {
    val monorepoRoot =
        buildscript.sourceFile?.parentFile?.parentFile?.parentFile?.parentFile?.parentFile ?: error("Cannot find products home")
    val pregeneratedMonorepoPath = monorepoRoot.resolve("Plugins/_ForTea.Pregenerated")
    ForTeaGeneratorSettings(
        pregeneratedMonorepoPath.resolve("BackendModel"),
        pregeneratedMonorepoPath.resolve("Frontend/src/com/jetbrains/fortea/model"),
        ".Pregenerated"
    )
} else {
    val riderBackendPluginPath = forTeaRepoRoot.resolve("Backend/RiderPlugin/ForTea.RiderPlugin")
    ForTeaGeneratorSettings(
        riderBackendPluginPath.resolve("Model"),
        forTeaRepoRoot.resolve("Frontend/src/main/kotlin/com/jetbrains/fortea/model"),
        ""
    )
}

rdgen {
    verbose = true
    packages = "model"

    generator {
        language = "kotlin"
        transform = "asis"
        root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
        namespace = "com.jetbrains.rider.model"
        directory = generatorOutputSettings.ktOutput.absolutePath
        generatedFileSuffix = generatorOutputSettings.suffix
    }

    generator {
        language = "csharp"
        transform = "reversed"
        root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
        namespace = "JetBrains.Rider.Model"
        directory = generatorOutputSettings.csOutput.absolutePath
        generatedFileSuffix = generatorOutputSettings.suffix
    }
}

tasks.withType<RdGenTask> {
    dependsOn(sourceSets["main"].runtimeClasspath)
    classpath(sourceSets["main"].runtimeClasspath)
}

dependencies {
    if (isMonorepo) {
        implementation(project(":rider-model"))
    } else {
        val rdVersion: String by project
        val rdKotlinVersion: String by project

        implementation("com.jetbrains.rd:rd-gen:$rdVersion")
        implementation("org.jetbrains.kotlin:kotlin-stdlib:$rdKotlinVersion")
        implementation(
            project(
                mapOf(
                    "path" to ":",
                    "configuration" to "riderModel"
                )
            )
        )
    }
}
