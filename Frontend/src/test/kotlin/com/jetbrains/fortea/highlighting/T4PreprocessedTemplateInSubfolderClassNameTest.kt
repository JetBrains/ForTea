package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4PreprocessedTemplateInSubfolderClassNameTest  : T4HighlightingTestBase() {
  override val testSolution: String = "ProjectWithComplexPreprocessedT4"
  override val testFilePath: String
    get() = "$testSolution/Folder/$fileName"

  @Test fun testClassName() = doTestErrors()
}