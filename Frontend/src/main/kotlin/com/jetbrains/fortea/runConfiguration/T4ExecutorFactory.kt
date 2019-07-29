package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.CantRunException
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.project.Project
import com.jetbrains.rider.run.configurations.IExecutorFactory
import com.jetbrains.rider.runtime.DotNetRuntime
import com.jetbrains.rider.runtime.RiderDotNetActiveRuntimeHost
import com.jetbrains.rider.util.idea.getComponent
import com.jetbrains.rider.util.idea.getLogger

class T4ExecutorFactory(project: Project, private val parameters: T4ConfigurationParameters) : IExecutorFactory {

  private val logger = getLogger<T4ExecutorFactory>()
  private val riderDotNetActiveRuntimeHost = project.getComponent<RiderDotNetActiveRuntimeHost>()

  override fun create(executorId: String, environment: ExecutionEnvironment): RunProfileState {
    val dotNetExecutable = parameters.toDotNetExecutable()
    val runtimeToExecute = DotNetRuntime.detectRuntimeForExeOrThrow(
      riderDotNetActiveRuntimeHost,
      dotNetExecutable.exePath,
      dotNetExecutable.useMonoRuntime
    )
    logger.info("Configuration will be executed on ${runtimeToExecute.javaClass.name}")
    return when (executorId) {
      DefaultRunExecutor.EXECUTOR_ID -> runtimeToExecute.createRunState(dotNetExecutable, environment)
      DefaultDebugExecutor.EXECUTOR_ID -> runtimeToExecute.createDebugState(dotNetExecutable, environment)
      else -> throw CantRunException("Unsupported executor $executorId")
    }
  }
}