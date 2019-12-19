package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4ComplexPreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithComplexPreprocessedT4"
  override val testFilePath: String
    get() = "${getSolutionDirectoryName()}/Folder/$fileName"

  @Test fun testPartials1() = doTestAll()
  @Test fun testPartials2() = doTestAll()
  @Test fun testPartials3() = doTestAll()
}