package com.jetbrains.fortea.lexer

import com.jetbrains.fortea.Tags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(Tags.Episode.ForTea)
class T4PreprocessedOutputDependentLexerTest : T4OutputDependentLexerTestBase() {
  override val testSolution = "ProjectWithSimplePreprocessedT4"

  @Test fun `test default extension in preprocessed template`() { doTest() }
}