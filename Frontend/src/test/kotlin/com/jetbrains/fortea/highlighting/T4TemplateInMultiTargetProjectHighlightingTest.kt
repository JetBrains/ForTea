package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.TestSettings
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import org.testng.annotations.Test

@TestSettings(sdkVersion = SdkVersion.LATEST_STABLE)
class T4TemplateInMultiTargetProjectHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithMultipleTargetFrameworks"
  override val testFilePath get() = "Project/$fileName"

  @Test fun testSimpleFile() = doTestErrors()
  @Test fun testFileWithMsBuildProperty() = doTestErrors()
  @Test fun testFunctionFromIncluder() = doTestErrors()
}