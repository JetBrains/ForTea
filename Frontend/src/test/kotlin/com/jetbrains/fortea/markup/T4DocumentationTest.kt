package com.jetbrains.fortea.markup

import com.jetbrains.fortea.daemon.T4HighlightingAttributeIds
import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.junit5.base.DocumentationTestBase
import com.jetbrains.rider.test.scriptingApi.`is`
import com.jetbrains.rider.test.scriptingApi.waitForDaemon
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea)
@Mute
@Solution("ProjectWithT4")
class T4DocumentationTest : DocumentationTestBase() {
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