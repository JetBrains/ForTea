package com.jetbrains.fortea.run

import com.intellij.execution.ExecutionManager
import com.intellij.execution.impl.ExecutionManagerImpl
import com.intellij.openapi.components.ServiceManager
import com.jetbrains.fortea.configuration.execution.impl.T4SynchronousRunConfigurationExecutor
import com.jetbrains.fortea.utils.T4TestHelper
import com.jetbrains.rdclient.util.idea.toVirtualFile
import com.jetbrains.rider.model.T4ExecutionRequest
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.base.BaseTestWithSolution
import com.jetbrains.rider.util.idea.getComponent
import org.testng.annotations.BeforeMethod

open class T4RunFileTestBase : BaseTestWithSolution() {
  override val waitForCaches = true
  override fun getSolutionDirectoryName() = testMethod.name

  protected lateinit var helper: T4TestHelper

  @BeforeMethod
  open fun setUp() {
    val manager = ServiceManager.getService(project, ExecutionManager::class.java) as ExecutionManagerImpl
    manager.forceCompilationInTests = true
    helper = createTestHelper()
  }

  protected open fun createTestHelper() = T4TestHelper(project)

  protected fun doTest(resultExtension: String? = null, dumpCsproj: Boolean = true) {
    executeT4File()
    helper.saveSolution(project)
    helper.dumpExecutionResult(resultExtension)
    if (dumpCsproj) helper.dumpCsprojContents()
  }

  protected fun testExecutionFailure(resultExtension: String, dumpCsproj: Boolean = false) {
    executeT4File()
    helper.saveSolution(project)
    helper.assertNoOutputWithExtension(resultExtension)
    if (dumpCsproj) helper.dumpCsprojContents()
  }

  private fun executeT4File() {
    val host = project.getComponent<ProjectModelViewHost>()
    val virtualFile = helper.t4File.path.toVirtualFile(true).shouldNotBeNull()
    val id = host.getItemsByVirtualFile(virtualFile).single().id
    val location = T4FileLocation(id)
    project.solution.t4ProtocolModel.prepareExecution.sync(location)
    val request = T4ExecutionRequest(location, false)
    T4SynchronousRunConfigurationExecutor(project, host) {
      !T4SynchronousRunConfigurationExecutor.isExecutionRunning
    }.execute(request)
  }
}
