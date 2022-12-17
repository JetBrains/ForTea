package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.HighlightSeverity
import com.intellij.openapi.editor.impl.EditorImpl
import com.jetbrains.fortea.inTests.T4TestHost
import com.jetbrains.rdclient.protocol.protocolHost
import com.jetbrains.rider.test.waitForDaemon
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

  fun doTest(severity: HighlightSeverity) = doTestWithMarkupModel {
    T4TestHost.getInstance(project!!.protocolHost).waitForIndirectInvalidation()
    waitForDaemon()
    dumpHighlightersTree(severity)
  }

  fun doTestErrors() = doTest(HighlightSeverity.ERROR)

  private fun doTestWithMarkupModel(testAction: EditorImpl.() -> Unit) =
    doTestWithMarkupModel(fileName, testFilePath, goldFileName, testAction)
}