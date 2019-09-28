package com.jetbrains.fortea.run

import com.intellij.execution.ExecutionManager
import com.intellij.execution.impl.ExecutionManagerKtImpl
import com.intellij.openapi.components.ServiceManager
import com.jetbrains.fortea.configuration.execution.impl.T4SynchronousRunConfigurationExecutor
import com.jetbrains.rdclient.util.idea.toVirtualFile
import com.jetbrains.rider.model.T4ExecutionRequest
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solutionDirectory
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.base.BaseTestWithSolution
import com.jetbrains.rider.test.framework.combine
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.util.idea.getComponent
import org.testng.annotations.BeforeMethod
import org.testng.annotations.Test

// Important note:
//   method names will at some point be parsed as program arguments,
//   so they cannot contain spaces!
@Test
class T4RunFileTest : BaseTestWithSolution() {
  override fun getSolutionDirectoryName() = testMethod.name
  override val waitForCaches = true

  private val t4File
    get() = project
      .solutionDirectory
      .combine(getSolutionDirectoryName())
      .listFiles { _, name -> name.endsWith(".tt") }
      .shouldNotBeNull()
      .single()

  private val outputFile
    get() = t4File
      .parentFile
      .listFiles()
      .shouldNotBeNull()
      .single {
        it.nameWithoutExtension == t4File.nameWithoutExtension
          && it.name != t4File.name
          && !it.name.endsWith(".tmp")
          && !it.name.endsWith(".gold")
      }

  private val csprojFile
    get() = t4File
      .parentFile
      .listFiles { _, name -> name.endsWith(".csproj") }
      .shouldNotBeNull()
      .single()

  @BeforeMethod
  fun setUp() {
    val manager = ServiceManager.getService(project, ExecutionManager::class.java) as ExecutionManagerKtImpl
    manager.forceCompilationInTests = true
  }

  @Test
  fun testThatFileCanBeExecuted() = doTest()

  private fun doTest() {
    executeT4File()
    dumpExecutionResult()
    // TODO: dump csproj, too
    // dumpCsproj()
  }

  private fun executeT4File() {
    val host = project.getComponent<ProjectModelViewHost>()
    val virtualFile = t4File.path.toVirtualFile(true).shouldNotBeNull()
    val id = host.getItemsByVirtualFile(virtualFile).single().id
    val request = T4ExecutionRequest(T4FileLocation(id), false)
    T4SynchronousRunConfigurationExecutor(project, host).execute(request)
  }

  private fun dumpCsproj() = executeWithGold(csprojFile.path) { it.print(csprojFile.readText()) }

  private fun dumpExecutionResult() = executeWithGold(t4File.path) { it.print(outputFile.readText()) }
}
