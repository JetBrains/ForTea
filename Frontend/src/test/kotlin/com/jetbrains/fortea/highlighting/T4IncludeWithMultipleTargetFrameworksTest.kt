package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.scriptingApi.checkSwea
import com.jetbrains.rider.test.scriptingApi.waitForDaemon
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea)
@Solution("IncludeInMultipleTargetFrameworks")
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