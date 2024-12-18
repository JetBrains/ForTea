package com.jetbrains.fortea.markup

import com.intellij.openapi.util.TextRange
import com.jetbrains.rider.test.scriptingApi.waitForDaemon
import com.jetbrains.rider.daemon.generated.ReSharperAttributesIds
import com.jetbrains.rider.test.base.DocumentationTestBase
import com.jetbrains.rider.test.scriptingApi.`is`
import org.testng.annotations.Ignore
import org.testng.annotations.Test

@Ignore
class T4CSharpDocumentationTest : DocumentationTestBase() {
  override val testSolution = "ProjectWithT4"

  @Test
  fun testCSharpVarToolTip() {
    doTestWithMarkupModel("Template.tt", "Template.tt") {
      waitForDaemon()
      dumpHighlightersTree({
        it `is` ReSharperAttributesIds.CSHARP_KEYWORD &&
          "var" == document.getText(it.textRange)
      }, {
        generateBackendHoverDoc(it.startOffset)
      })
    }
  }
}