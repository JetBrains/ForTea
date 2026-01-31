package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.scriptingApi.checkSwea
import com.jetbrains.rider.test.scriptingApi.waitForDaemon
import org.testng.annotations.Test

class T4IncludeWithMultipleTargetFrameworksTest : T4HighlightingTestBase() {
  override val testSolution = "IncludeInMultipleTargetFrameworks"
  override val fileName = "Include.ttinclude"
  override val checkSolutionLoad = false


  @Test
  fun `test that there are no errors in solution`() = doTestWithMarkupModelNoGold(fileName, testFilePath) {
    waitForDaemon()
    checkSwea(project!!, 0)
  }
}