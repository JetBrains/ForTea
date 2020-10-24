package com.jetbrains.fortea.markup

import com.intellij.codeInsight.documentation.DocumentationManager
import com.intellij.psi.PsiManager
import com.jetbrains.fortea.daemon.T4HighlightingAttributeIds
import com.jetbrains.rdclient.quickDoc.FrontendDocumentationProvider
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.test.base.BaseTestWithMarkup
import com.jetbrains.rider.test.base.`is`
import org.testng.annotations.Test

class T4HoverDocTest : BaseTestWithMarkup() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testMacroToolTip() {
    withSynchronousPlainHoverDoc {
      doTestWithMarkupModel("Template.tt", "Template.tt") {
        val editor = this
        waitForDaemon()
        dumpHighlightersTree({ it `is` T4HighlightingAttributeIds.MACRO }, {
          val psiFile = PsiManager.getInstance(project!!).findFile(editor.virtualFile)!!
          editor.caretModel.moveToOffset(it.startOffset)
          val originalElement = psiFile.findElementAt(editor.caretModel.offset)!!
          val element = DocumentationManager.getInstance(project!!).findTargetElement(editor, psiFile, originalElement)
          val documentationProvider = DocumentationManager.getProviderFromElement(element)
          val doc = documentationProvider.generateHoverDoc(element ?: originalElement, originalElement)
            ?: return@dumpHighlightersTree null
          assert(doc.endsWith('/') || doc.endsWith('\\'))
          val docWithoutTrailingSlash = doc.substring(0, doc.lastIndex)
          ".../" + docWithoutTrailingSlash.substring(maxOf(
            docWithoutTrailingSlash.lastIndexOf('/') + 1,
            docWithoutTrailingSlash.lastIndexOf('\\') + 1
          ))
        })
      }
    }
  }

  private fun withSynchronousPlainHoverDoc(block: () -> Unit) {
    val oldPump = FrontendDocumentationProvider.PUMP_DURING_SPIN
    val oldConverter = FrontendDocumentationProvider.RICH_TEXT_CONVERTER
    try {
      FrontendDocumentationProvider.PUMP_DURING_SPIN = true
      FrontendDocumentationProvider.RICH_TEXT_CONVERTER = { tooltipModel ->
        tooltipModel.richText.parts.joinToString("") { it.text }
      }
      block()
    } finally {
      FrontendDocumentationProvider.PUMP_DURING_SPIN = oldPump
      FrontendDocumentationProvider.RICH_TEXT_CONVERTER = oldConverter
    }
  }
}