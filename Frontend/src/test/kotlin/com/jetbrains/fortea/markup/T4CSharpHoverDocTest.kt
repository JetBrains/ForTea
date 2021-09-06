package com.jetbrains.fortea.markup

import com.intellij.openapi.editor.impl.EditorImpl
import com.intellij.openapi.util.TextRange
import com.jetbrains.rd.ide.model.quickDocHostModel
import com.jetbrains.rdclient.quickDoc.RichTextHtmlUtils.highlightAttributedParts
import com.jetbrains.rdclient.services.IdeBackend
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.daemon.generated.ReSharperAttributesIds
import com.jetbrains.rider.test.base.HoverDocTestBase
import com.jetbrains.rider.test.scriptingApi.`is`
import com.jetbrains.rider.test.framework.flushQueues
import org.testng.annotations.Ignore
import org.testng.annotations.Test
import kotlin.test.assertNotNull

@Ignore
class T4CSharpHoverDocTest : HoverDocTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testCSharpVarToolTip() {
    doTestWithMarkupModel("Template.tt", "Template.tt") {
      waitForDaemon()
      dumpHighlightersTree({
        it `is` ReSharperAttributesIds.CSHARP_KEYWORD &&
          "var" == document.getText(TextRange(it.startOffset, it.endOffset))
      }, {
        generateBackendHoverDoc_(it.startOffset)
      })
    }
  }

  // TODO: use the method from SDK once it's there
  private fun EditorImpl.generateBackendHoverDoc_(offset: Int? = null): String {
    val project = project
    assertNotNull(project, "No project found")
    val quickDocHostModel = IdeBackend.getInstance(project).solution.quickDocHostModel
    quickDocHostModel.startHoverDocSession.sync(offset ?: caretModel.offset)
    flushQueues()
    val session = IdeBackend.getInstance(project).solution.quickDocHostModel.hoverDocSession.value
    assertNotNull(session, "Hover doc calculation failed")
    val result = session.initialInfo.descriptionHtml.highlightAttributedParts()
    quickDocHostModel.hoverDocSession.set(null)
    return result
  }
}