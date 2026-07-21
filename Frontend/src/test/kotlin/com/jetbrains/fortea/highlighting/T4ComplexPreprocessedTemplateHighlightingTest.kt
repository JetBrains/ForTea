package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.HighlightSeverity
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea)
@Solution("ProjectWithComplexPreprocessedT4")
class T4ComplexPreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override val testSolution: String= "ProjectWithComplexPreprocessedT4"
  override val testFilePath: String
    get() = "$testSolution/Folder/$fileName"

  @Test fun testPartials1() = doTest(HighlightSeverity.ERROR)
  @Test fun testPartials2() = doTest(HighlightSeverity.ERROR)
  @Test fun testPartials3() = doTest(HighlightSeverity.ERROR)
}