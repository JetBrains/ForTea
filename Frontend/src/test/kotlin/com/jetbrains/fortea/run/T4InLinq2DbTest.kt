package com.jetbrains.fortea.run

import org.testng.annotations.Test

class T4InLinq2DbTest : T4RunFileTestBase() {
  override val restoreNuGetPackages = true
  @Test
  fun testDefaultLinq2DbTemplate() = doTest(".generated.cs")
}