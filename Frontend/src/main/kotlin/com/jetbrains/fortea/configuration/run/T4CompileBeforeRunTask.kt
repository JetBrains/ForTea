package com.jetbrains.fortea.configuration.run

import com.intellij.execution.BeforeRunTask
import com.intellij.execution.BeforeRunTaskProvider
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.actionSystem.DataContext
import com.intellij.openapi.util.Key
import com.intellij.util.concurrency.Semaphore
import com.jetbrains.fortea.configuration.T4BuildSessionView
import com.jetbrains.rider.model.T4BuildResultKind
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.util.idea.getComponent
import com.jetbrains.rider.util.idea.lifetime

class T4CompileBeforeRunTask : BeforeRunTask<T4CompileBeforeRunTask>(CompileT4BeforeRunTaskProvider.providerId)

class CompileT4BeforeRunTaskProvider : BeforeRunTaskProvider<T4CompileBeforeRunTask>() {
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
    if (executionRequest.isVisible) view.openWindow(project.lifetime)
    val finished = Semaphore()
    finished.down()
    var successful = false

    val host = ProjectModelViewHost.getInstance(project)
    val location = executionRequest.location
    val item = host.getItemById(location.id) ?: return false
    val path = item.getVirtualFile()?.path ?: return false
    val model = project.solution.t4ProtocolModel

    val request = model.requestCompilation.start(location).result
    request.advise(project.lifetime) { rdTaskResult ->
      try {
        val result = rdTaskResult.unwrap()
        successful = result.buildResultKind.isSuccess
        if (executionRequest.isVisible) view.showT4BuildResult(project.lifetime, result.messages, path)
      } finally {
        finished.up()
      }
    }

    finished.waitFor()
    if (!successful) model.executionAborted.fire(location)
    return successful
  }

  companion object {
    val providerId = Key.create<T4CompileBeforeRunTask>("Compile T4")

    private val T4BuildResultKind.isSuccess
      get() = when (this) {
        T4BuildResultKind.HasErrors -> false
        T4BuildResultKind.HasWarnings -> true
        T4BuildResultKind.Successful -> true
      }
  }
}
