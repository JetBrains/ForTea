import org.jetbrains.grammarkit.GrammarKitConstants
import org.jetbrains.grammarkit.tasks.GenerateLexerTask
import org.jetbrains.grammarkit.tasks.GenerateParserTask
import java.nio.file.Files
import java.nio.file.StandardCopyOption

plugins {
    id("org.jetbrains.grammarkit")
}

repositories {
    maven("https://cache-redirector.jetbrains.com/intellij-dependencies")
    maven("https://cache-redirector.jetbrains.com/maven-central")
}

val isMonorepo = rootProject.projectDir != projectDir.parentFile
val forTeaRepoRoot: File = projectDir.parentFile.parentFile

val platformLibConfiguration by configurations.registering
val flexConfiguration by configurations.registering
val grammarKitConfiguration by configurations.registering

@Suppress("UnstableApiUsage")
dependencies {
  flexConfiguration("org.jetbrains.intellij.deps.jflex:jflex:${GrammarKitConstants.JFLEX_DEFAULT_VERSION}")
  grammarKitConfiguration("org.jetbrains:grammar-kit:${GrammarKitConstants.GRAMMAR_KIT_DEFAULT_VERSION}")
  platformLibConfiguration(project(
    mapOf(
      "path" to ":",
      "configuration" to "platformLibConfiguration"
    )
  ))
}

data class ForTeaGeneratorSettings(val parserOutput: File, val lexerOutput: File)

val forTeaGeneratorSettings = if (isMonorepo) {
    val monorepoRoot = buildscript.sourceFile?.parentFile?.parentFile?.parentFile?.parentFile?.parentFile?.parentFile ?: error("Monorepo root not found")
    check(monorepoRoot.resolve(".ultimate.root.marker").isFile) {
        error("Incorrect location in monorepo: monorepoRoot='$monorepoRoot'")
    }

    val pregeneratedMonorepoPath = monorepoRoot.resolve("dotnet/Plugins/_ForTea.Pregenerated")
    ForTeaGeneratorSettings(
        pregeneratedMonorepoPath.resolve("Frontend/src"),
        pregeneratedMonorepoPath.resolve("Frontend/src/com/jetbrains/fortea/lexer")
    )
} else {
    ForTeaGeneratorSettings(
        forTeaRepoRoot.resolve("Frontend/src/generated/java"),
        forTeaRepoRoot.resolve("Frontend/src/generated/java/com/jetbrains/fortea/lexer")
    )
}
tasks {
    create<GenerateParserTask>("generateParser") {
        inputs.files(platformLibConfiguration)

        sourceFile.set(file("src/parser/T4.bnf"))
        targetRoot.set(forTeaGeneratorSettings.parserOutput.absolutePath)
        purgeOldFiles.set(true)
        pathToParser.set("fakePathToParser") // I have no idea what should be inserted here, but this works
        pathToPsiRoot.set("fakePathToPsiRoot") // same
        classpath(grammarKitConfiguration)

        doFirst {
            val libFile = platformLibConfiguration.get().singleFile
            val libPath = File(libFile.readText().trim())
            val jarFiles = libPath.listFiles()?.filter { it.extension == "jar" }.orEmpty()
            classpath(jarFiles)
        }

        doLast {
            forTeaGeneratorSettings.parserOutput.walk().filter { it.isFile }.forEach {
                normalizeLineEndings(it)
            }
        }
    }

    create<GenerateLexerTask>("generateLexer") {
        sourceFile.set(file("src/lexer/_T4Lexer.flex"))
        targetDir.set(forTeaGeneratorSettings.lexerOutput.absolutePath)
        val targetName = "_T4Lexer"
        targetClass.set(targetName)
        purgeOldFiles.set(true)
        classpath(flexConfiguration)

        doLast {
            val targetFile = File(targetDir.get()).resolve("$targetName.java")
            if (!targetFile.exists()) error("Lexer file $targetFile was not generated")
            normalizeLineEndings(targetFile)
        }
    }
}

fun normalizeLineEndings(file: File) {
    val tempFile = File.createTempFile("${file.name}.temp", null)
    file.useLines { lines ->
        tempFile.bufferedWriter().use { writer ->
            lines.forEach { line ->
                // rewrite the line with LF
                writer.write(line + "\n")
            }
        }
    }
    Files.move(tempFile.toPath(), file.toPath(), StandardCopyOption.REPLACE_EXISTING)
}
