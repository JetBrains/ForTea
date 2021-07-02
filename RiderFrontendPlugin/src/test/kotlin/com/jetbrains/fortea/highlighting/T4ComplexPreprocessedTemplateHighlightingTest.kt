package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.HighlightSeverity
import org.testng.annotations.Test

class T4ComplexPreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithComplexPreprocessedT4"
  override val testFilePath: String
    get() = "${getSolutionDirectoryName()}/Folder/$fileName"

  @Test fun testPartials1() = doTest(HighlightSeverity.ERROR)
  @Test fun testPartials2() = doTest(HighlightSeverity.ERROR)
  @Test fun testPartials3() = doTest(HighlightSeverity.ERROR)
}