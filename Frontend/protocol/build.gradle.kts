plugins {
  id("java")
  kotlin("jvm")
  id("com.jetbrains.rdgen")
}

dependencies {
  compile("org.jetbrains.kotlin:kotlin-stdlib")
  compile(group = "com.jetbrains.rd", name = "rd-gen")
  compile(files("""${rootProject.projectDir}/build/riderRD-2019.2-SNAPSHOT/lib/rider.jar"""))
}

repositories {
  mavenCentral()
}
