rootProject.name = "ForTea"

pluginManagement {
  repositories {
    gradlePluginPortal()
    // This is for RdGen plugin.
    maven { setUrl("https://cache-redirector.jetbrains.com/www.myget.org/F/rd-snapshots/maven") }
    // This is for snapshot version of 'org.jetbrains.intellij' plugin
    maven { setUrl("https://oss.sonatype.org/content/repositories/snapshots/") }
  }
  resolutionStrategy {
    eachPlugin {
      when(requested.id.name) {
        // This required to correctly rd-gen plugin resolution. May be we should switch our naming to match Gradle plugin naming convention.
        "rdgen" -> {
          useModule("com.jetbrains.rd:rd-gen:${requested.version}")
        }
      }
    }
  }
}