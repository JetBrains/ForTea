package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4PreprocessedTemplateInSubfolderClassNameTest  : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithComplexPreprocessedT4"
  override val testFilePath: String
    get() = "${getSolutionDirectoryName()}/Folder/$fileName"

  @Test fun testClassName() = doTestErrors()
}