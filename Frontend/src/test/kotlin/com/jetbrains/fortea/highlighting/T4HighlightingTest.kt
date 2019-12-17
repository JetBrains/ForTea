package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.daemon.T4RunMarkerAttributeIds
import org.testng.annotations.Test

class T4HighlightingTest : T4HighlightingTestBase() {
  @Test fun testGutterMarks() = doTest(T4RunMarkerAttributeIds.RUN_T4_FILE_MARKER_ID)
}