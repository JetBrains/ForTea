package com.jetbrains.fortea.lexer

import org.testng.annotations.Test

class T4PreprocessedOutputDependentLexerTest : T4OutputDependentLexerTestBase() {
  override val testSolution = "ProjectWithSimplePreprocessedT4"

  @Test fun `test default extension in preprocessed template`() = doTest()
}