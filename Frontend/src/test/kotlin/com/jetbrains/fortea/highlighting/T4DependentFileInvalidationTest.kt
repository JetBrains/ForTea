package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.HighlightSeverity
import com.intellij.openapi.editor.impl.EditorImpl
import com.jetbrains.rdclient.daemon.IFrontendDocumentMarkupAdapter
import com.jetbrains.rdclient.daemon.components.FrontendMarkupHost
import com.jetbrains.rdclient.testFramework.typeWithLatency
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.daemon.util.annotateDocumentWithHighlighterTags
import com.jetbrains.rider.daemon.util.severity
import com.jetbrains.rider.test.base.EditorTestBase
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.scriptingApi.commitBackendPsiFiles
import com.jetbrains.rider.test.scriptingApi.setCaretAfterWord
import com.jetbrains.rider.test.scriptingApi.withOpenedEditor
import org.testng.annotations.Ignore
import org.testng.annotations.Test
import java.io.File
import java.io.PrintStream

/**
 * This class duplicates some logic from BaseTestWithMarkup.
 * However, that class cannot be used because its API is not rich enough to cover our case:
 * we need to open files in editor _without_ running write actions and overriding their contents.
 * This is what this test is all about.
 *
 * I made some changes in that API and will supposedly merge them into net202.
 * TODO: reuse platform code here as soon as that code is available in SDK
 * TODO: can I remove waitForDaemon() calls?
 */
@Ignore("TODO: find out why it fails. Possibly a race")
class T4DependentFileInvalidationTest : EditorTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithManyTemplates"
  private val projectName = "ProjectWithManyTemplates"

  override val traceCategories = listOf("#com.jetbrains.rider.daemon", "JetBrains.ReSharper.Host.Features.Daemon")
  private var _printStream: PrintStream? = null
  private val printStream get() = _printStream ?: error("Use doTestWithMarkupModel for tests with dumping")
  override val checkTextControls = false

  @Test
  fun `test that a change in include triggers includer invalidation`() {
    doTestWithMarkupModel("$projectName/TemplateWithFunctionUsage.tt", "TemplateWithFunctionUsage_before.gold") {
      waitForDaemon()
      dumpHighlightersTree(HighlightSeverity.ERROR)
    }
    withOpenedEditor("$projectName/IncludeWithFunction.ttinclude") {
      setCaretAfterWord("Foo")
      typeWithLatency("1")
    }
    doTestWithMarkupModel("$projectName/TemplateWithFunctionUsage.tt", "TemplateWithFunctionUsage_after.gold") {
      waitForDaemon()
      dumpHighlightersTree(HighlightSeverity.ERROR)
    }
  }

  @Test
  fun `test that a change in includer triggers include invalidation`() {
    doTestWithMarkupModel("$projectName/IncludeWithFunction.ttinclude", "IncludeWithFunction_before.gold") {
      waitForDaemon()
      dumpHighlightersTree()
    }
    withOpenedEditor("$projectName/TemplateWithFunctionUsage.tt") {
      setCaretAfterWord("Foo")
      typeWithLatency("1")
      commitBackendPsiFiles()
    }
    doTestWithMarkupModel("$projectName/IncludeWithFunction.ttinclude", "IncludeWithFunction_after.gold") {
      waitForDaemon()
      dumpHighlightersTree()
    }
  }

  @Test
  fun `test that a change in a file triggers indirect include invalidation`() {
    doTestWithMarkupModel("$projectName/Directory/IncludeWithUsage.ttinclude", "IncludeWithUsage_before.gold") {
      waitForDaemon()
      dumpHighlightersTree(HighlightSeverity.ERROR)
    }
    withOpenedEditor("$projectName/IncludeWithOtherFunction.ttinclude") {
      setCaretAfterWord("Bar")
      typeWithLatency("1")
    }
    doTestWithMarkupModel("$projectName/Directory/IncludeWithUsage.ttinclude", "IncludeWithUsage_after.gold") {
      waitForDaemon()
      dumpHighlightersTree(HighlightSeverity.ERROR)
    }
  }

  private fun doTestWithMarkupModel(testFilePath: String, goldFileName: String, testAction: EditorImpl.() -> Unit) {
    withOpenedEditor(testFilePath) {
      val goldFile = File(testCaseGoldDirectory, goldFileName).apply { if (!exists()) { parentFile.mkdirs(); } }
      executeWithGold(goldFile) { stream: PrintStream ->
        _printStream = stream
        testAction(this)
        _printStream = null
      }
    }
  }

  private fun EditorImpl.dumpHighlightersTree(severity: HighlightSeverity) =
    printStream.print(annotateDocumentWithHighlighterTags(markupAdapter, { highlighter ->
      highlighter.severity?.let { it.myVal >= severity.myVal } ?: false
    }))
  private fun EditorImpl.dumpHighlightersTree() =
    printStream.print(annotateDocumentWithHighlighterTags(markupAdapter, { true }))

  private val EditorImpl.markupAdapter: IFrontendDocumentMarkupAdapter
    get() = FrontendMarkupHost.getMarkupContributor(project!!, document)!!.markupAdapter
}