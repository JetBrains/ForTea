package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4PreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithPreprocessedT4"

  @Test fun testPartials() = doTestErrors()
}