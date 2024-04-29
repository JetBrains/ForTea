plugins {
    id("org.jetbrains.grammarkit")
    id("org.jetbrains.intellij")
}

repositories {
    maven("https://cache-redirector.jetbrains.com/intellij-dependencies")
    maven("https://cache-redirector.jetbrains.com/maven-central")
}

val riderVersion: String by project
intellij {
    version.set("$riderVersion-SNAPSHOT")
    type.set("RD")
}

val isMonorepo = rootProject.projectDir != projectDir.parentFile
val forTeaRepoRoot: File = projectDir.parentFile.parentFile

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
    generateParser.configure {
        sourceFile.set(file("src/parser/T4.bnf"))
        targetRoot.set(forTeaGeneratorSettings.parserOutput.absolutePath)
        purgeOldFiles.set(true)
        pathToParser.set("fakePathToParser") // I have no idea what should be inserted here, but this works
        pathToPsiRoot.set("fakePathToPsiRoot") // same
        classpath(setupDependencies.flatMap { it.idea.map { idea -> idea.classes.resolve("lib/opentelemetry.jar") } })
    }

    generateLexer.configure {
        sourceFile.set(file("src/lexer/_T4Lexer.flex"))
        targetDir.set(forTeaGeneratorSettings.lexerOutput.absolutePath)
        targetClass.set("_T4Lexer")
        purgeOldFiles.set(true)
    }
}

