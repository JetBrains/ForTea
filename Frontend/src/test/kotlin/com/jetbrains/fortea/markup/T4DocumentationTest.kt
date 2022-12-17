package com.jetbrains.fortea.markup

import com.jetbrains.fortea.daemon.T4HighlightingAttributeIds
import com.jetbrains.rider.test.waitForDaemon
import com.jetbrains.rider.test.base.DocumentationTestBase
import com.jetbrains.rider.test.scriptingApi.`is`
import org.testng.annotations.Ignore
import org.testng.annotations.Test

@Ignore
class T4DocumentationTest : DocumentationTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testMacroToolTip() {
    doTestWithMarkupModel("Template.tt", "Template.tt") {
      waitForDaemon()
      dumpHighlightersTree({ it `is` T4HighlightingAttributeIds.MACRO }, {
        val doc = generateBackendHoverDoc(it.startOffset)
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
}