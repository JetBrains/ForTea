package com.jetbrains.fortea.run

import com.jetbrains.rider.test.OpenSolutionParams
import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test
import java.time.Duration

@Tag(TeamCityTags.Plugins.ForTea.General)
class T4InLinq2DbTest : T4RunFileTestBase() {
  override fun modifyOpenSolutionParams(params: OpenSolutionParams) {
    super.modifyOpenSolutionParams(params)
    params.restoreNuGetPackages = true
    params.backendLoadedTimeout = Duration.ofMinutes(10)
  }

  @Mute("RIDER-98543")
  @Solution("testDefaultLinq2DbTemplate")
  @Test
  fun testDefaultLinq2DbTemplate() = doTest(".generated.cs")
}