package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.daemon.T4RunMarkerAttributeIds
import org.testng.annotations.Test

class T4ExecutableTemplateHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithT4"

  @Test fun testGutterMarks() = doTest(T4RunMarkerAttributeIds.RUN_T4_FILE_MARKER_ID)
  @Test fun testClass() = doTestErrors()
  @Test fun testIncompleteMacro() = doTestErrors()
  @Test fun `test that default base class has TransformText`() = doTestErrors()
  @Test fun `test that default base class implements IDisposable`() = doTestErrors()
  @Test fun `test generated class name`() = doTestErrors()
}