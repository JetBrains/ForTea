package com.jetbrains.fortea.highlighting

import org.testng.annotations.Ignore
import org.testng.annotations.Test

class T4PreprocessedFileWithSharedIncludeHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithPreprocessedT4IncludedWithPartial"
  override val fileName = "Include.ttinclude"
  override val testFilePath: String
    get() = "Project/$fileName"

  @Ignore("TODO: implement test")
  @Test
  fun `test swea in template included into multiple files`() = doTestErrors()
}