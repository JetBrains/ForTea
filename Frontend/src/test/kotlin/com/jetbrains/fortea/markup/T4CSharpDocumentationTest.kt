package com.jetbrains.fortea.markup

import com.jetbrains.rider.daemon.generated.ReSharperAttributesIds
import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.junit5.base.DocumentationTestBase
import com.jetbrains.rider.test.scriptingApi.`is`
import com.jetbrains.rider.test.scriptingApi.waitForDaemon
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea.General)
@Mute
@Solution("ProjectWithT4")
class T4CSharpDocumentationTest : DocumentationTestBase() {
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