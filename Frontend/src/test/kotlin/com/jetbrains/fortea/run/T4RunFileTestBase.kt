package com.jetbrains.fortea.run

import com.intellij.execution.ExecutionManager
import com.intellij.execution.impl.ExecutionManagerKtImpl
import com.intellij.openapi.components.ServiceManager
import com.jetbrains.fortea.configuration.execution.impl.T4SynchronousRunConfigurationExecutor
import com.jetbrains.rdclient.util.idea.toVirtualFile
import com.jetbrains.rider.ideaInterop.vfs.VfsWriteOperationsHost
import com.jetbrains.rider.model.T4ExecutionRequest
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.projectView.solutionDirectory
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.base.BaseTestWithSolution
import com.jetbrains.rider.test.framework.combine
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.framework.flushQueues
import com.jetbrains.rider.test.scriptingApi.waitAllCommandsFinished
import com.jetbrains.rider.test.scriptingApi.waitForProjectModelReady
import com.jetbrains.rider.util.idea.application
import com.jetbrains.rider.util.idea.getComponent
import org.testng.annotations.BeforeMethod
import java.io.File

open class T4RunFileTestBase : BaseTestWithSolution() {
  override val waitForCaches = true
  override fun getSolutionDirectoryName() = testMethod.name

  protected open val t4File: File
    get() = project
      .solutionDirectory
      .combine("Project")
      .listFiles { _, name -> name.endsWith(".tt") or name.endsWith(".t4") }
      .shouldNotBeNull()
      .single()

  private val csprojFile
    get() = t4File
      .parentFile
      .listFiles { _, name -> name.endsWith(".csproj") }
      .shouldNotBeNull()
      .single()

  private val outputFileCandidates
    get() = t4File.parentFile.listFiles().shouldNotBeNull()

  private fun findOutputFile(resultExtension: String?): File {
    return if (resultExtension == null) outputFileCandidates.single {
      it.nameWithoutExtension == t4File.nameWithoutExtension
        && it.name != t4File.name
        && !it.name.endsWith(".tmp")
        && !it.name.endsWith(".gold")
    } else outputFileCandidates.single {
      it.name == t4File.nameWithoutExtension + resultExtension
    }
  }

  @BeforeMethod
  fun setUp() {
    val manager = ServiceManager.getService(project, ExecutionManager::class.java) as ExecutionManagerKtImpl
    manager.forceCompilationInTests = true
  }

  protected fun doTest(resultExtension: String? = null, dumpCsproj: Boolean = true) {
    executeT4File()
    saveSolution()
    dumpExecutionResult(resultExtension)
    if (dumpCsproj) dumpCsproj()
  }

  protected fun executeT4File() {
    val host = project.getComponent<ProjectModelViewHost>()
    val virtualFile = t4File.path.toVirtualFile(true).shouldNotBeNull()
    val id = host.getItemsByVirtualFile(virtualFile).single().id
    val location = T4FileLocation(id)
    project.solution.t4ProtocolModel.prepareExecution.sync(location)
    val request = T4ExecutionRequest(location, false)
    T4SynchronousRunConfigurationExecutor(project, host) {
      !T4SynchronousRunConfigurationExecutor.isExecutionRunning
    }.execute(request)
  }

  protected fun saveSolution() {
    application.saveAll()
    flushQueues()
    waitForProjectModelReady(project)
    waitAllCommandsFinished()
    project.getComponent<VfsWriteOperationsHost>().waitRefreshIsFinished()
  }

  protected fun dumpCsproj() = executeWithGold(csprojFile.path) {
    it.print(csprojFile.readText())
  }

  protected fun dumpExecutionResult(resultExtension: String? = null) = executeWithGold(t4File.path) {
    it.print(findOutputFile(resultExtension).readText())
  }

  protected fun assertNoOutputWithExtension(extension: String) = assert(outputFileCandidates.none {
    it.nameWithoutExtension == t4File.nameWithoutExtension && it.name.endsWith(extension)
  })
}
