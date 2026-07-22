package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea)
@Solution("ProjectWithSimplePreprocessedT4")
class T4SimplePreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override val checkSolutionLoad = false

  @Test
  fun testVisibility() = doTestErrors()
}