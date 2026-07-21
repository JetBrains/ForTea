package com.jetbrains.fortea.run

import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.annotations.TestSettings
import com.jetbrains.rider.test.enums.BuildTool
import com.jetbrains.rider.test.enums.Mono
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

// Note: due to Windows path length restriction
// test method name cannot be longer than 60 symbols
@Mute("RIDER-98455")
@Tag(TeamCityTags.Plugins.ForTea)
@TestSettings(sdkVersion = SdkVersion.LATEST_STABLE, buildTool = BuildTool.SDK, mono = Mono.UNIX_ONLY)
internal class T4EnvDTETest : T4RunFileTestBase() {
  @Solution("test that host specific template can access EnvDTE")
  @Test fun `test that host specific template can access EnvDTE`() = doTest(dumpCsproj = false)
  @Solution("test basic DTE functions")
  @Test fun `test basic DTE functions`() = doTest(dumpCsproj = false)
  @Solution("test solution functions")
  @Test fun `test solution functions`() = doTest(dumpCsproj = false)
  @Solution("test project functions")
  @Test fun `test project functions`() = doTest(dumpCsproj = false)
  @Solution("test AST functions")
  @Test fun `test AST functions`() = doTest(dumpCsproj = false)
}
