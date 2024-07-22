package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4SimplePreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithSimplePreprocessedT4"
  override val checkSolutionLoad = false

  @Test
  fun testVisibility() = doTestErrors()
}