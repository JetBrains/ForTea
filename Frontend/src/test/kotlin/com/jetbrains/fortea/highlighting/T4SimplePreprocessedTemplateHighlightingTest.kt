package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Test

@Solution("ProjectWithSimplePreprocessedT4")
class T4SimplePreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithSimplePreprocessedT4"
  override val checkSolutionLoad = false

  @Test
  fun testVisibility() = doTestErrors()
}