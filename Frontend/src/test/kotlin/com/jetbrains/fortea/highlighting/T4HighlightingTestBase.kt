package com.jetbrains.fortea.highlighting

import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.test.base.BaseTestWithMarkup

abstract class T4HighlightingTestBase : BaseTestWithMarkup() {
  final override fun getSolutionDirectoryName() = "ProjectWithT4"
  private val fileName = "Template.tt"

  fun doTest(attributeId: String) = doTestWithMarkupModel(fileName, "${getSolutionDirectoryName()}/$fileName", "$fileName.gold") {
    waitForDaemon()
    dumpHighlightersTree(attributeId)
  }
}