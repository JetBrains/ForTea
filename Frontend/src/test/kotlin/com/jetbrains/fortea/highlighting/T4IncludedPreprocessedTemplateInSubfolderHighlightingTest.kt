package com.jetbrains.fortea.highlighting

import org.testng.annotations.Ignore
import org.testng.annotations.Test

class T4IncludedPreprocessedTemplateInSubfolderHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4IncludedInSubfolder"
  override val testFilePath: String
    get() = "${getSolutionDirectoryName()}/Folder/$fileName"

  @Ignore("FIXME")
  @Test
  fun testClassName() = doTestErrors()
}