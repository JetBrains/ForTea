package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.impl.EditorImpl
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.test.base.BaseTestWithMarkup

abstract class T4HighlightingTestBase : BaseTestWithMarkup() {
  abstract override fun getSolutionDirectoryName(): String
  protected open val fileName get() = "Template.tt"
  protected open val goldFileName get() = "$fileName.gold"
  protected open val testFilePath get() = "${getSolutionDirectoryName()}/$fileName"

  fun doTest(attributeId: String) = doTestWithMarkupModel {
    waitForDaemon()
    dumpHighlightersTree(attributeId)
  }

  fun doTestAll() = doTestWithMarkupModel {
    waitForDaemon()
    dumpHighlightersTree()
  }

  private fun doTestWithMarkupModel(testAction: EditorImpl.() -> Unit) =
    doTestWithMarkupModel(fileName, testFilePath, goldFileName, testAction)
}