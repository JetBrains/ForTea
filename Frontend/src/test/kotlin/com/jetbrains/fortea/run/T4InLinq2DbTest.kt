package com.jetbrains.fortea.run

import org.testng.annotations.Test
import java.time.Duration

class T4InLinq2DbTest : T4RunFileTestBase() {
  override val restoreNuGetPackages = true
  override val backendLoadedTimeout: Duration = Duration.ofMinutes(10)

  @Test
  fun testDefaultLinq2DbTemplate() = doTest(".generated.cs")
}