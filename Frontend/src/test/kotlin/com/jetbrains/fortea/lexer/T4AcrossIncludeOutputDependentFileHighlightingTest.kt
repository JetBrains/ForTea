package com.jetbrains.fortea.lexer

import com.jetbrains.rider.test.scriptingApi.withOpenedEditor
import org.testng.annotations.Test

class T4AcrossIncludeOutputDependentFileHighlightingTest : T4OutputDependentLexerTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4AndIncluder"

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

  @Test
  fun `test that a change in includer can trigger include invalidation`() {
    setText(includerName, "$includerName.before")
    val includeEditor = doTest(includeName, "$includeName.before.gold")
    setText(includerName, "$includerName.after")
    doTest(includeEditor, "$includeName.after.gold")
  }

  @Test
  fun `test that a change in include can trigger includer invalidation`() {
    setText(includeName, "$includeName.before")
    val includerEditor = doTest(includerName, "$includerName.before.gold")
    setText(includeName, "$includeName.after")
    doTest(includerEditor, "$includerName.after.gold")
  }

  private fun setText(fileName: String, sourceFileName: String? = null) =
    withOpenedEditor(fileName, sourceFileName ?: fileName) {}

  companion object {
    private const val includeName = "Template.tt"
    private const val includerName = "Includer.tt"
  }
}