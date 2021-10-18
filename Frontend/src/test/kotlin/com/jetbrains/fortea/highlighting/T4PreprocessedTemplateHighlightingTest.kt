package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4PreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithPreprocessedT4"

  @Test fun testPartials() = doTestErrors()
  @Test fun `test that default base class has no TransformText`() = doTestErrors()
  @Test fun `test that default base class does not implement IDisposable`() = doTestErrors()
  // https://youtrack.jetbrains.com/issue/RIDER-60147
  @Test fun `test that FormatProvider is mutable`() = doTestErrors()
  @Test fun `test generated class name`() = doTestErrors()
}