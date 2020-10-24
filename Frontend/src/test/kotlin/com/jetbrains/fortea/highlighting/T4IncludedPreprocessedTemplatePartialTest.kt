package com.jetbrains.fortea.highlighting

import org.testng.annotations.Ignore
import org.testng.annotations.Test

class T4IncludedPreprocessedTemplatePartialTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithPreprocessedT4IncludedWithPartial"
  override val fileName = "Include.ttinclude"
  override val testFilePath: String
    get() = "Project/$fileName"

  @Ignore("TODO: figure out how to wait for PSI module rebuild")
  @Test
  fun `test partial class resolution in preprocessed include`() = doTestErrors()

  @Ignore("TODO: figure out how to wait for PSI module rebuild")
  @Test
  fun `test that TransformText is resolved in preprocessed include`() = doTestErrors()
}