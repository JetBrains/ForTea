package com.jetbrains.fortea.lexer

import com.jetbrains.rider.test.base.BaseTestWithSolution
import org.testng.annotations.Test

@Test
class T4RunFileTest : BaseTestWithSolution() {
  override fun getSolutionDirectoryName() = "T4BasicExecution"
  override val waitForCaches = true

  @Test
  fun `test that file can be executed`() {
    val project1 = this.project
    // doTestDumpProjectsView {
    //   dump("Init", true) {
    //   }
    // }
  }
}
