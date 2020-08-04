package com.jetbrains.fortea.lexer

import com.jetbrains.rider.test.scriptingApi.withOpenedEditor
import org.testng.annotations.Test

class T4AcrossIncludeOutputDependentFileHighlightingTest : T4OutputDependentLexerTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4AndIncluder"
  private val includeName = "Template.tt"
  private val includerName = "Includer.tt"

  @Test
  fun `test that extension in include defines highlighting in includer`() {
    setText(includeName)
    doTest(includerName)
  }

  @Test
  fun `test that extension in includer defines highlighting in include`() {
    setText(includerName)
    doTest(includeName)
  }

  private fun setText(fileName: String) {
    withOpenedEditor(fileName, fileName) {}
  }
}