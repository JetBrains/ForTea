package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.TestSettings
import com.jetbrains.rider.test.enums.BuildTool
import com.jetbrains.rider.test.enums.Mono
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea.General)
@TestSettings(sdkVersion = SdkVersion.LATEST_STABLE, buildTool = BuildTool.SDK, mono = Mono.UNIX_ONLY)
@Solution("ProjectWithMultipleTargetFrameworks")
class T4TemplateInMultiTargetProjectHighlightingTest : T4HighlightingTestBase() {
  override val testFilePath get() = "Project/$fileName"

  @Test fun testSimpleFile() = doTestErrors()
  @Test fun testFileWithMsBuildProperty() = doTestErrors()
  @Test fun testFunctionFromIncluder() = doTestErrors()
}