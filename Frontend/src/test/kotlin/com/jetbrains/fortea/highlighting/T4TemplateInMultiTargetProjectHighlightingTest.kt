package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.TestEnvironment
import com.jetbrains.rider.test.env.enums.SdkVersion
import org.testng.annotations.Test

@TestEnvironment(sdkVersion = SdkVersion.LATEST_STABLE)
class T4TemplateInMultiTargetProjectHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithMultipleTargetFrameworks"
  override val testFilePath get() = "Project/$fileName"

  @Test fun testSimpleFile() = doTestErrors()
  @Test fun testFileWithMsBuildProperty() = doTestErrors()
  @Test fun testFunctionFromIncluder() = doTestErrors()
}