package com.jetbrains.fortea.markup

import com.intellij.codeInsight.daemon.impl.HighlightInfo
import com.intellij.openapi.editor.impl.EditorImpl
import com.intellij.openapi.editor.markup.RangeHighlighter
import com.jetbrains.fortea.daemon.T4HighlightingAttributeIds
import com.jetbrains.rdclient.daemon.highlighters.tooltips.FrontendInTestTooltipProvider
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.daemon.generated.ReSharperAttributesIds
import com.jetbrains.rider.test.base.BaseTestWithMarkup
import com.jetbrains.rider.test.base.`is`
import org.testng.annotations.Test

class T4ToolTipTest : BaseTestWithMarkup() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testMacroToolTip() = doTest {
    // Cannot dump the tooltips themselves here, because they are system-dependent
    dumpHighlightersTree({
      it `is` T4HighlightingAttributeIds.MACRO &&
        !(it.errorStripeTooltip as? HighlightInfo)?.description.isNullOrEmpty()
    })
  }

  private fun doTest(block: EditorImpl.() -> Unit) = withSynchronousTooltips {
    doTestWithMarkupModel("Template.tt", "Template.tt") {
      waitForDaemon()
      block()
    }
  }


  private fun withSynchronousTooltips(isSyncTooltips: Boolean = true, block: () -> Unit) {
    val oldValue = FrontendInTestTooltipProvider.SYNCHRONOUS_TOOLTIPS
    try {
      FrontendInTestTooltipProvider.SYNCHRONOUS_TOOLTIPS = isSyncTooltips
      block()
    } finally {
      FrontendInTestTooltipProvider.SYNCHRONOUS_TOOLTIPS = oldValue
    }
  }
}