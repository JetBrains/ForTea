package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.daemon.T4RunMarkerAttributeIds
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea.General)
@Solution("ProjectWithT4")
class T4ExecutableTemplateHighlightingTest : T4HighlightingTestBase() {
  @Test fun testGutterMarks() = doTest(T4RunMarkerAttributeIds.RUN_T4_FILE_MARKER_ID)
  @Test fun testClass() = doTestErrors()
  @Test fun testIncompleteMacro() = doTestErrors()
  @Test fun `test that default base class has TransformText`() = doTestErrors()
  @Test fun `test that default base class implements IDisposable`() = doTestErrors()
  @Test fun `test generated class name`() = doTestErrors()
}