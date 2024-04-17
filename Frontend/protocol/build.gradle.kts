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

val pair = if (isMonorepo) {
    val productsHome = buildscript.sourceFile?.parentFile?.parentFile?.parentFile?.parentFile ?: error("Cannot find products home")
    val pregeneratedMonorepoPath = productsHome.resolve("Plugins/_ForTea.Pregenerated")
    pregeneratedMonorepoPath.resolve("BackendModel") to
            pregeneratedMonorepoPath.resolve("Frontend/src/com/jetbrains/fortea/model")
} else {
    val riderBackendPluginPath = forTeaRepoRoot.resolve("Backend/RiderPlugin/ForTea.RiderPlugin")
    riderBackendPluginPath.resolve("Model") to
            forTeaRepoRoot.resolve("Frontend/src/main/kotlin/com/jetbrains/fortea/model")
}
val csOutput = pair.first
val ktOutput = pair.second

val suffix = if (isMonorepo) {
    ".Pregenerated"
} else {
    ""
}

rdgen {
    verbose = true
    packages = "model"

    generator {
        language = "kotlin"
        transform = "asis"
        root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
        namespace = "com.jetbrains.rider.model"
        directory = ktOutput.absolutePath
        generatedFileSuffix = suffix
    }

    generator {
        language = "csharp"
        transform = "reversed"
        root = "com.jetbrains.rider.model.nova.ide.IdeRoot"
        namespace = "JetBrains.Rider.Model"
        directory = csOutput.absolutePath
        generatedFileSuffix = suffix
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
