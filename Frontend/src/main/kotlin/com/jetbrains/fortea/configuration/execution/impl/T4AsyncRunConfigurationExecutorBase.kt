package com.jetbrains.fortea.configuration.execution.impl

import com.intellij.execution.*
import com.intellij.execution.process.NopProcessHandler
import com.intellij.execution.runners.ExecutionEnvironmentBuilder
import com.intellij.openapi.project.Project

abstract class T4AsyncRunConfigurationExecutorBase(project: Project) : T4RunConfigurationExecutorBase(project) {
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
