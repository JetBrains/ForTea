package com.jetbrains.fortea.configuration.execution.impl

import com.intellij.execution.*
import com.intellij.execution.process.NopProcessHandler
import com.intellij.execution.runners.ExecutionEnvironmentBuilder
import com.intellij.openapi.project.Project
import com.jetbrains.rider.projectView.ProjectModelViewHost

abstract class T4AsyncRunConfigurationExecutorBase(
  project: Project,
  host: ProjectModelViewHost
) : T4RunConfigurationExecutorBase(project, host) {
  protected abstract val executor: Executor

  final override fun executeConfiguration(configuration: RunnerAndConfigurationSettings) {
    val builder: ExecutionEnvironmentBuilder
    try {
      builder = ExecutionEnvironmentBuilder.create(executor, configuration)
    } catch (e: ExecutionException) {
      return
    }

    val environment = builder.contentToReuse(null).dataContext(null).activeTarget().build()
    ProgramRunnerUtil.executeConfigurationAsync(
      environment,
      true,
      true
    ) {
      val listener = project.messageBus.syncPublisher(ExecutionManager.EXECUTION_TOPIC)
      listener.processStarted(executor.id, environment, NopProcessHandler())
    }
  }
}
