package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.daemon.T4RunMarkerAttributeIds
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rider.test.base.BaseTestWithMarkup
import org.testng.annotations.Test

class T4HighlightingTest : BaseTestWithMarkup() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testGutterMarks() = doTestWithMarkupModel("Template.tt", "ProjectWithT4/Template.tt", "Template.tt.gold") {
    waitForDaemon()
    dumpHighlightersTree(T4RunMarkerAttributeIds.RUN_T4_FILE_MARKER_ID)
  }
}