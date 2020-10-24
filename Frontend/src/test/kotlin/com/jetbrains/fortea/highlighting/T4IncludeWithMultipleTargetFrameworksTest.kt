package com.jetbrains.fortea.highlighting

import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.test.scriptingApi.checkSwea
import org.testng.annotations.Test

class T4IncludeWithMultipleTargetFrameworksTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "IncludeInMultipleTargetFrameworks"
  override val fileName = "Include.ttinclude"

  @Test
  fun `test that there are no errors in solution`() = doTestWithMarkupModelNoGold(fileName, testFilePath) {
    waitForDaemon()
     checkSwea(project!!, 0)
  }
}