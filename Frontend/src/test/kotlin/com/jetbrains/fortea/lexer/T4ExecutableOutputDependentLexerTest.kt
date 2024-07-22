package com.jetbrains.fortea.lexer

import org.testng.annotations.Test

class T4ExecutableOutputDependentLexerTest : T4OutputDependentLexerTestBase() {
  override val testSolution = "ProjectWithT4"

  @Test fun `test basic C# highlighting`() = doTest()
  @Test fun `test basic HTML highlighting`() = doTest()
  @Test fun `test that extension can contain dot`() = doTest()
  @Test fun `test default extension in executable template`() = doTest()
}