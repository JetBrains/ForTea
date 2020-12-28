package com.jetbrains.fortea.run

import com.intellij.openapi.application.PathManager
import com.jetbrains.rider.test.framework.combine
import org.testng.annotations.Test
import java.time.Duration

class T4InLinq2DbTest : T4RunFileTestBase() {
  override val restoreNuGetPackages = true
  override val backendLoadedTimeout: Duration = Duration.ofMinutes(10)

  @Test
  fun testDefaultLinq2DbTemplate() {
    try {
      doTest(".generated.cs")
    }
    finally {
      val target = java.io.File(PathManager.getLogPath()).combine("testDefaultLinq2DbTemplate")
      if (target.exists()) target.deleteRecursively()
      java.io.File(project.basePath).copyRecursively(target)
    }
  }
}