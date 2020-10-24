package com.jetbrains.fortea.configuration.run.task

import com.intellij.execution.BeforeRunTaskProvider
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.actionSystem.DataContext
import com.intellij.openapi.util.Key
import com.intellij.util.concurrency.Semaphore
import com.jetbrains.fortea.configuration.T4BuildSessionView
import com.jetbrains.fortea.configuration.isSuccess
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
import com.jetbrains.fortea.utils.handleEndOfExecution
import com.jetbrains.rd.platform.util.getComponent
import com.jetbrains.fortea.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rd.platform.util.lifetime

class T4CompileBeforeRunTaskProvider : BeforeRunTaskProvider<T4CompileBeforeRunTask>() {
  override fun getName() = "Compile T4 File"

  override fun getId(): Key<T4CompileBeforeRunTask> = providerId

  override fun createTask(runConfiguration: RunConfiguration): T4CompileBeforeRunTask? {
    if (runConfiguration !is T4RunConfiguration) return null
    val task = T4CompileBeforeRunTask()
    task.isEnabled = true
    return task
  }

  override fun executeTask(
    context: DataContext,
    configuration: RunConfiguration,
    env: ExecutionEnvironment,
    task: T4CompileBeforeRunTask
  ): Boolean {
    if (configuration !is T4RunConfiguration) return false

    val project = configuration.project
    val view = project.getComponent<T4BuildSessionView>()
    val executionRequest = configuration.parameters.request
    if (executionRequest.isVisible) view.openWindow("T4 Build Started...")
    val finished = Semaphore()
    finished.down()
    var successful = false

    val host = ProjectModelViewHost.getInstance(project)
    val location = executionRequest.location
    val item = host.getItemById(location.id) ?: return false
    val path = item.getVirtualFile()?.path ?: return false
    val model = project.solution.t4ProtocolModel

    model.requestCompilation.start(project.lifetime, location).result.advise(project.lifetime) { rdTaskResult ->
      try {
        val result = rdTaskResult.unwrap()
        successful = result.buildResultKind.isSuccess
        if (executionRequest.isVisible) view.showT4BuildResult(result, path)
      } finally {
        finished.up()
      }
    }

    finished.waitFor()
    if (!successful) model.executionAborted.handleEndOfExecution(location)
    return successful
  }

  companion object {
    val providerId = Key.create<T4CompileBeforeRunTask>("Compile T4")
  }
}
