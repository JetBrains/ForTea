import com.jetbrains.rd.generator.gradle.RdgenParams

buildscript {
  repositories {
    maven { setUrl("https://cache-redirector.jetbrains.com/www.myget.org/F/rd-snapshots/maven") }
    mavenCentral()
  }
  dependencies {
    classpath("com.jetbrains.rd:rd-gen:0.192.36")
  }
}

plugins {
  id("org.jetbrains.intellij") version "0.4.9"
}

apply {
  plugin("com.jetbrains.rdgen")
}

version = "0.01"

intellij {
  type = "RD"
  version = "2019.2"
  instrumentCode = false
  downloadSources = false
  updateSinceUntilBuild = false
  // Workaround for https://youtrack.jetbrains.com/issue/IDEA-179607
  setPlugins("rider-plugins-appender")
}

configure<RdgenParams> {
  hashFolder = "build/rdgen"

  classpath({
    logger.info("Calculating classpath for rdgen, intellij.ideaDependency is ${intellij.ideaDependency}")
    val sdkPath = intellij.ideaDependency.classes
    val rdLibDirectory = File(sdkPath, "lib/rd").canonicalFile
    "$rdLibDirectory/rider-model.jar"
  })

  sources(File(projectDir, "model"))
  packages = "model"

  generator {
    language = "csharp"
    transform = "asis"
    root = "model.T4SubprocessProtocolRoot"
    namespace = "JetBrains.Rider.Model"
    directory = "${File(projectDir, "SubprocessProtocol")}"
  }

  generator {
    language = "csharp"
    transform = "reversed"
    root = "model.T4SubprocessProtocolRoot"
    namespace = "JetBrains.Rider.Model"
    directory = "${File(projectDir, "PluginProtocol")}"
  }
}

defaultTasks("rdgen")
