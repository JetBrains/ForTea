package com.jetbrains.fortea.configuration.run.task

import com.intellij.execution.BeforeRunTaskProvider
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.actionSystem.DataContext
import com.intellij.openapi.util.Key
import com.intellij.util.concurrency.Semaphore
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
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
    var result = false

    val project = configuration.project
    val model = project.solution.t4ProtocolModel

    model.requestCompilation.start(configuration.parameters.initialFilePath).result.advise(project.lifetime) { rdTaskResult ->
      try {
        result = rdTaskResult.unwrap()
      } finally {
        finished.up()
      }
    }

    finished.waitFor()
    return result
  }

  companion object {
    val providerId = Key.create<CompileT4BeforeRunTask>("Compile T4")
  }
}
