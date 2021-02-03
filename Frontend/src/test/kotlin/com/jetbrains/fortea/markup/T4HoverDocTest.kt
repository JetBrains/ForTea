package com.jetbrains.fortea.markup

import com.intellij.openapi.editor.impl.EditorImpl
import com.jetbrains.fortea.daemon.T4HighlightingAttributeIds
import com.jetbrains.rd.ide.model.quickDocHostModel
import com.jetbrains.rdclient.quickDoc.RichTextHtmlUtils.highlightAttributedParts
import com.jetbrains.rdclient.services.IdeBackend
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.test.base.BaseTestWithMarkup
import com.jetbrains.rider.test.base.HoverDocTestBase
import com.jetbrains.rider.test.base.`is`
import com.jetbrains.rider.test.framework.flushQueues
import org.testng.annotations.Ignore
import org.testng.annotations.Test
import kotlin.test.assertNotNull

@Ignore
class T4HoverDocTest : HoverDocTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testMacroToolTip() {
    doTestWithMarkupModel("Template.tt", "Template.tt") {
      waitForDaemon()
      dumpHighlightersTree({ it `is` T4HighlightingAttributeIds.MACRO }, {
        val doc = generateBackendHoverDoc_(it.startOffset)
        assert(doc.endsWith('/') || doc.endsWith('\\'))
        val docWithoutTrailingSlash = doc.substring(0, doc.lastIndex)
        ".../" + docWithoutTrailingSlash.substring(
          maxOf(
            docWithoutTrailingSlash.lastIndexOf('/') + 1,
            docWithoutTrailingSlash.lastIndexOf('\\') + 1
          )
        )
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