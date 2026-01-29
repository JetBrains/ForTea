package com.jetbrains.fortea.run

import com.jetbrains.rider.test.annotations.TestSettings
import com.jetbrains.rider.test.enums.BuildTool
import com.jetbrains.rider.test.enums.Mono
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import org.testng.annotations.Test

// Note: due to Windows path length restriction
// test method name cannot be longer than 60 symbols
@TestSettings(sdkVersion = SdkVersion.LATEST_STABLE, buildTool = BuildTool.SDK, mono = Mono.UNIX_ONLY)
internal class T4EnvDTETest : T4RunFileTestBase() {
  @Test fun `test that host specific template can access EnvDTE`() = doTest(dumpCsproj = false)
  @Test fun `test basic DTE functions`() = doTest(dumpCsproj = false)
  @Test fun `test solution functions`() = doTest(dumpCsproj = false)
  @Test fun `test project functions`() = doTest(dumpCsproj = false)
  @Test fun `test AST functions`() = doTest(dumpCsproj = false)
}
