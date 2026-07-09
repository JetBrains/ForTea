package com.jetbrains.fortea.lexer

import com.jetbrains.fortea.Tags
import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(Tags.Episode.ForTea)
@Solution("ProjectWithSimplePreprocessedT4")
class T4PreprocessedOutputDependentLexerTest : T4OutputDependentLexerTestBase() {
  override val testSolution = "ProjectWithSimplePreprocessedT4"
  override val checkSolutionLoad = false

  @Test fun `test default extension in preprocessed template`() { doTest() }
}