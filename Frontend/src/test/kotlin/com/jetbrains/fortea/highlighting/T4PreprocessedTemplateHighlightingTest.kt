package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4PreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithPreprocessedT4"

  @Test fun testPartials() = doTestErrors()
  @Test fun `test that default base class has no TransformText`() = doTestErrors()
  @Test fun `test that default base class does not implement IDisposable`() = doTestErrors()
}