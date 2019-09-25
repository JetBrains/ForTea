package com.jetbrains.fortea.lexer

import com.jetbrains.fortea.configuration.T4RunConfigurationCreator
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.base.BaseTestWithSolution
import com.jetbrains.rider.util.idea.getComponent
import org.testng.annotations.Test

@Test
class T4RunFileTest : BaseTestWithSolution() {
  override fun getSolutionDirectoryName() = "T4BasicExecution"
  override val waitForCaches = true

  @Test
  fun `test that file can be executed`() {
    val t4File = project.projectFile?.parent?.children?.single { it.extension == "tt" }.shouldNotBeNull()
    val runConfigurationCreator = project.getComponent<T4RunConfigurationCreator>()
    runConfigurationCreator.launchExecution(t4File)
  }
}
