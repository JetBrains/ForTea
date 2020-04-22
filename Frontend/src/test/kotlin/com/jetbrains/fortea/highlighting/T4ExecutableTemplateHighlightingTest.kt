package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.daemon.T4RunMarkerAttributeIds
import org.testng.annotations.Test

class T4ExecutableTemplateHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test fun testGutterMarks() = doTest(T4RunMarkerAttributeIds.RUN_T4_FILE_MARKER_ID)
  @Test fun testClass() = doTestAll()
  @Test fun testIncompleteMacro() = doTestErrors()
}