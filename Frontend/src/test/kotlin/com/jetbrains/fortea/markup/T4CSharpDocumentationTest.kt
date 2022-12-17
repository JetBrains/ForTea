package com.jetbrains.fortea.markup

import com.intellij.openapi.util.TextRange
import com.jetbrains.rider.test.waitForDaemon
import com.jetbrains.rider.daemon.generated.ReSharperAttributesIds
import com.jetbrains.rider.test.base.DocumentationTestBase
import com.jetbrains.rider.test.scriptingApi.`is`
import org.testng.annotations.Ignore
import org.testng.annotations.Test

@Ignore
class T4CSharpDocumentationTest : DocumentationTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testCSharpVarToolTip() {
    doTestWithMarkupModel("Template.tt", "Template.tt") {
      waitForDaemon()
      dumpHighlightersTree({
        it `is` ReSharperAttributesIds.CSHARP_KEYWORD &&
          "var" == document.getText(TextRange(it.startOffset, it.endOffset))
      }, {
        generateBackendHoverDoc(it.startOffset)
      })
    }
  }
}