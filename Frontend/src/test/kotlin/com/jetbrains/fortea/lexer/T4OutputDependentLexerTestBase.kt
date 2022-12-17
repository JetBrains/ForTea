package com.jetbrains.fortea.lexer

import com.intellij.openapi.editor.impl.EditorImpl
import com.intellij.openapi.util.TextRange
import com.jetbrains.rider.test.waitForDaemon
import com.jetbrains.rider.test.base.EditorTestBase
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.framework.waitBackend
import com.jetbrains.rider.test.scriptingApi.waitForDaemonAndCaches
import com.jetbrains.rider.test.scriptingApi.withOpenedEditor
import java.io.File

abstract class T4OutputDependentLexerTestBase : EditorTestBase() {
  protected fun doTest(fileName: String = "Template.tt", goldFileName: String? = null) =
    withOpenedEditor(fileName, fileName) { doTest(this, goldFileName ?: "$fileName.gold") }

  protected fun doTest(editor: EditorImpl, goldFileName: String, defaultWait: ((editor: EditorImpl) -> Unit)? = null) {
    val wait = defaultWait ?: {
      waitForDaemonAndCaches(editor.project!!)
      waitBackend()
      editor.waitForDaemon()
      waitBackend()
    }
    val goldFile = File(testCaseGoldDirectory, goldFileName)
    if (!goldFile.exists()) goldFile.parentFile.mkdirs()
    executeWithGold(goldFile) { stream ->
      wait(editor)
      val iterator = editor.highlighter.createIterator(0)
      while (!iterator.atEnd()) {
        val tokenName = iterator.tokenType.toString().extendLength(40)
        val tokenText = editor.document.getText(TextRange(iterator.start, iterator.end)).replace("\n", "\\n")
        stream.println("$tokenName| $tokenText")
        iterator.advance()
      }
    }
  }

  private fun String.extendLength(targetLength: Int): String {
    if (length >= targetLength) return this
    val builder = StringBuilder()
    builder.append(this)
    while (builder.length < targetLength) {
      builder.append(' ')
    }
    return builder.toString()
  }
}