package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.Tags
import com.jetbrains.rider.test.annotations.TestSettings
import com.jetbrains.rider.test.enums.BuildTool
import com.jetbrains.rider.test.enums.Mono
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(Tags.Episode.ForTea)
@TestSettings(sdkVersion = SdkVersion.LATEST_STABLE, buildTool = BuildTool.SDK, mono = Mono.UNIX_ONLY)
@Solution("ProjectWithMultipleTargetFrameworks")
class T4TemplateInMultiTargetProjectHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithMultipleTargetFrameworks"
  override val testFilePath get() = "Project/$fileName"

  @Test fun testSimpleFile() = doTestErrors()
  @Test fun testFileWithMsBuildProperty() = doTestErrors()
  @Test fun testFunctionFromIncluder() = doTestErrors()
}