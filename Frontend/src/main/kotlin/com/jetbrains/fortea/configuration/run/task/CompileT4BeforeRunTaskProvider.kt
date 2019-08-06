package com.jetbrains.fortea.configuration.run.task

import com.intellij.execution.BeforeRunTaskProvider
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.actionSystem.DataContext
import com.intellij.openapi.util.Key
import com.intellij.util.concurrency.Semaphore
import com.jetbrains.fortea.configuration.T4BuildSessionView
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
import com.jetbrains.rider.model.T4BuildResultKind
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.util.idea.lifetime

class CompileT4BeforeRunTaskProvider : BeforeRunTaskProvider<CompileT4BeforeRunTask>() {
  override fun getName() = "Compile T4 file"

  override fun getId(): Key<CompileT4BeforeRunTask> = providerId

  override fun createTask(runConfiguration: RunConfiguration): CompileT4BeforeRunTask? {
    if (runConfiguration !is T4RunConfiguration) return null
    val task = CompileT4BeforeRunTask()
    task.isEnabled = true
    return task
  }

  override fun executeTask(
    context: DataContext,
    configuration: RunConfiguration,
    env: ExecutionEnvironment,
    task: CompileT4BeforeRunTask
  ): Boolean {
    if (configuration !is T4RunConfiguration) return false

    val finished = Semaphore()
    finished.down()
    var successful = false

    val project = configuration.project
    val model = project.solution.t4ProtocolModel

    val request = model.requestCompilation.start(configuration.parameters.initialFilePath).result
    request.advise(project.lifetime) { rdTaskResult ->
      try {
        val result = rdTaskResult.unwrap()
        successful = result.buildResultKind != T4BuildResultKind.HasErrors
        val view = project.getComponent(T4BuildSessionView::class.java)
        view.showT4BuildResult(project.lifetime, result.messages, configuration.parameters.initialFilePath)
      } finally {
        finished.up()
      }
    }

    finished.waitFor()
    return successful
  }

  companion object {
    val providerId = Key.create<CompileT4BeforeRunTask>("Compile T4")
  }
}
