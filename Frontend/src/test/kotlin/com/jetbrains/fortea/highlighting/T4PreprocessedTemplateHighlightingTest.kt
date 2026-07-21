package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea)
@Solution("ProjectWithPreprocessedT4")
class T4PreprocessedTemplateHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithPreprocessedT4"

  @Test fun testPartials() = doTestErrors()
  @Test fun `test that default base class has no TransformText`() = doTestErrors()
  @Test fun `test that default base class does not implement IDisposable`() = doTestErrors()
  // https://youtrack.jetbrains.com/issue/RIDER-60147
  @Test fun `test that FormatProvider is mutable`() = doTestErrors()
  @Test fun `test generated class name`() = doTestErrors()
}