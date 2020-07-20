package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.scriptingApi.checkSwea
import org.testng.annotations.Test

class T4IncludeWithMultipleTargetFrameworksTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "IncludeInMultipleTargetFrameworks"
  override val fileName = "Include.ttinclude"

  @Test
  fun test() {
     checkSwea(project, 0)
  }
}