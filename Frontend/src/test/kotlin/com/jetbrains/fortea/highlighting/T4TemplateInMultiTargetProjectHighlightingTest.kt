package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.TestEnvironment
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import org.testng.annotations.Test

@TestEnvironment(sdkVersion = SdkVersion.LATEST_STABLE)
class T4TemplateInMultiTargetProjectHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithMultipleTargetFrameworks"
  override val testFilePath get() = "Project/$fileName"

  @Test fun testSimpleFile() = doTestErrors()
  @Test fun testFileWithMsBuildProperty() = doTestErrors()
  @Test fun testFunctionFromIncluder() = doTestErrors()
}