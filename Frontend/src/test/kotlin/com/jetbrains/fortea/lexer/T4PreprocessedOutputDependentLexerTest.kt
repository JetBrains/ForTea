package com.jetbrains.fortea.lexer

import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea.General)
@Solution("ProjectWithSimplePreprocessedT4")
class T4PreprocessedOutputDependentLexerTest : T4OutputDependentLexerTestBase() {
  override val checkSolutionLoad = false

  @Test fun `test default extension in preprocessed template`() { doTest() }
}