package com.jetbrains.fortea.markup

import com.intellij.codeInsight.documentation.DocumentationManager
import com.intellij.openapi.util.TextRange
import com.intellij.psi.PsiManager
import com.jetbrains.rdclient.quickDoc.FrontendDocumentationProvider
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.daemon.generated.ReSharperAttributesIds
import com.jetbrains.rider.test.base.BaseTestWithMarkup
import com.jetbrains.rider.test.base.`is`
import org.testng.annotations.Test

class T4CSharpHoverDocTest : BaseTestWithMarkup() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testCSharpVarToolTip() {
    withSynchronousPlainHoverDoc {
      doTestWithMarkupModel("Template.tt", "Template.tt") {
        val editor = this
        waitForDaemon()
        dumpHighlightersTree({
          it `is` ReSharperAttributesIds.CSHARP_KEYWORD &&
            "var" == document.getText(TextRange(it.startOffset, it.endOffset))
        }, {
          val psiFile = PsiManager.getInstance(project!!).findFile(editor.virtualFile)!!
          editor.caretModel.moveToOffset(it.startOffset)
          val originalElement = psiFile.findElementAt(editor.caretModel.offset)!!
          val element = DocumentationManager.getInstance(project!!).findTargetElement(editor, psiFile, originalElement)
          val documentationProvider = DocumentationManager.getProviderFromElement(element)
          documentationProvider.generateHoverDoc(element ?: originalElement, originalElement)
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