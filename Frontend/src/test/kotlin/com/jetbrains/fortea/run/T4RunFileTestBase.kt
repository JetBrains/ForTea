package com.jetbrains.fortea.run

import com.intellij.execution.ExecutionManager
import com.intellij.execution.impl.ExecutionManagerKtImpl
import com.intellij.openapi.components.ServiceManager
import com.jetbrains.fortea.configuration.execution.impl.T4SynchronousRunConfigurationExecutor
import com.jetbrains.rdclient.util.idea.toVirtualFile
import com.jetbrains.rider.ideaInterop.vfs.VfsWriteOperationsHost
import com.jetbrains.rider.model.T4ExecutionRequest
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.projectView.ProjectModelViewHost
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

  private val t4File
    get() = project
      .solutionDirectory
      .combine("Project")
      .listFiles { _, name -> name.endsWith(".tt") }
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
      it.nameWithoutExtension == t4File.nameWithoutExtension
        && it.name.endsWith(resultExtension)
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

  private fun executeT4File() {
    val host = project.getComponent<ProjectModelViewHost>()
    val virtualFile = t4File.path.toVirtualFile(true).shouldNotBeNull()
    val id = host.getItemsByVirtualFile(virtualFile).single().id
    val request = T4ExecutionRequest(T4FileLocation(id), false)
    T4SynchronousRunConfigurationExecutor(project, host, ::isExecutionFinished).execute(request)
  }

  private val isExecutionFinished: Boolean
    get() = outputFileCandidates.any {
      it.nameWithoutExtension == t4File.nameWithoutExtension
        && it.name != t4File.name
        && !it.name.endsWith(".tmp")
        && !it.name.endsWith(".gold")
    }

  private fun saveSolution() {
    application.saveAll()
    waitForProjectModelReady(project)
    flushQueues()
    waitAllCommandsFinished()
    project.getComponent<VfsWriteOperationsHost>().waitRefreshIsFinished()
  }

  private fun dumpCsproj() = executeWithGold(csprojFile.path) {
    // Note:
    //   in tests, template execution starts directly,
    //   not via backend 'execute template' action.
    //   Template type meanwhile is only updated from that action.
    //   This is why in csproj file <Generator> tag will be missing
    it.print(csprojFile.readText())
  }

  private fun dumpExecutionResult(resultExtension: String?) = executeWithGold(t4File.path) {
    it.print(findOutputFile(resultExtension).readText())
  }
}
