package com.jetbrains.fortea.run

import com.jetbrains.rider.test.annotations.Mute
import org.testng.annotations.Test
import java.time.Duration

class T4InLinq2DbTest : T4RunFileTestBase() {
  override val restoreNuGetPackages = true
  override val backendLoadedTimeout: Duration = Duration.ofMinutes(10)

  @Mute("RIDER-98543")
  @Test
  fun testDefaultLinq2DbTemplate() = doTest(".generated.cs")
}