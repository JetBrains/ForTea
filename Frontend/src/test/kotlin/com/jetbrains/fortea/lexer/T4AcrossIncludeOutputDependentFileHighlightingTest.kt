package com.jetbrains.fortea.lexer

import com.intellij.openapi.editor.impl.EditorImpl
import com.jetbrains.rider.test.framework.waitBackend
import com.jetbrains.rider.test.scriptingApi.waitForDaemon
import com.jetbrains.rider.test.scriptingApi.waitForDaemonAndCaches
import com.jetbrains.rider.test.scriptingApi.withOpenedEditor
import org.testng.annotations.Ignore
import org.testng.annotations.Test

@Ignore("Broken")
class T4AcrossIncludeOutputDependentFileHighlightingTest : T4OutputDependentLexerTestBase() {
  override val testSolution = "ProjectWithT4AndIncluder"

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
    doTestLast(includeEditor, "$includeName.after.gold")
  }

  @Test
  fun `test that a change in include can trigger includer invalidation`() {
    setText(includeName, "$includeName.before")
    val includerEditor = doTest(includerName, "$includerName.before.gold")
    setText(includeName, "$includeName.after")
    doTestLast(includerEditor, "$includerName.after.gold")
  }

  private fun doTestLast(editor: EditorImpl, goldFileName: String) = doTest(editor, goldFileName) {
    waitForDaemonAndCaches(it.project!!)
    waitBackend()
  }

  private fun setText(fileName: String, sourceFileName: String? = null) =
    withOpenedEditor(fileName, sourceFileName ?: fileName) { waitForDaemon() }

  companion object {
    private const val includeName = "Template.tt"
    private const val includerName = "Includer.tt"
  }
}