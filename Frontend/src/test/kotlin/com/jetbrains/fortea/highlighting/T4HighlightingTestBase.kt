package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.HighlightSeverity
import com.intellij.openapi.editor.impl.EditorImpl
import com.intellij.openapi.vfs.newvfs.impl.VfsRootAccess
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.test.base.BaseTestWithMarkup
import com.jetbrains.rider.test.base.PrepareTestEnvironment
import org.testng.annotations.BeforeSuite

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
    waitForDaemon()
    dumpHighlightersTree(severity)
  }

  @Deprecated("Dumping all highlighters causes flackiness", ReplaceWith("doTestErrors()"))
  fun doTestAll() = doTestWithMarkupModel {
    waitForDaemon()
    dumpHighlightersTree()
  }

  fun doTestErrors() = doTestWithMarkupModel {
    waitForDaemon()
    dumpHighlightersTree(HighlightSeverity.ERROR)
  }

  private fun doTestWithMarkupModel(testAction: EditorImpl.() -> Unit) =
    doTestWithMarkupModel(fileName, testFilePath, goldFileName, testAction)

//  @BeforeSuite(dependsOnMethods = ["initApplication"])
//  fun postInitApplication() =
//    VfsRootAccess.allowRootAccess(project, PrepareTestEnvironment.dotnetCoreCliPath)
}