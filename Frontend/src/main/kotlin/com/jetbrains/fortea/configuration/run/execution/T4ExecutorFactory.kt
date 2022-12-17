package com.jetbrains.fortea.configuration.run.execution

import com.intellij.execution.CantRunException
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.fortea.model.t4ProtocolModel
import com.jetbrains.rd.platform.util.getComponent
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.run.configurations.IExecutorFactory
import com.jetbrains.rider.runtime.DotNetRuntime
import com.jetbrains.rider.runtime.RiderDotNetActiveRuntimeHost

class T4ExecutorFactory(project: Project, private val parameters: T4RunConfigurationParameters) : IExecutorFactory {
  private val riderDotNetActiveRuntimeHost = project.getComponent<RiderDotNetActiveRuntimeHost>()
  override fun create(executorId: String, environment: ExecutionEnvironment) =
    createAsync(executorId, environment)

  fun createAsync(executorId: String, environment: ExecutionEnvironment): RunProfileState {
    val dotNetExecutable = parameters.toDotNetExecutable()
    val runtimeToExecute = DotNetRuntime.detectRuntimeForExeOrThrow(
      riderDotNetActiveRuntimeHost,
      dotNetExecutable.exePath,
      dotNetExecutable.runtimeType,
      dotNetExecutable.projectTfm
    )
    val model = environment.project.solution.t4ProtocolModel
    return when (executorId) {
      DefaultRunExecutor.EXECUTOR_ID -> {
        val wrappee = runtimeToExecute.createRunState(dotNetExecutable, environment)
        T4RunProfileWrapperState(wrappee, model, parameters)
      }
      DefaultDebugExecutor.EXECUTOR_ID -> {
        val wrappee = runtimeToExecute.createDebugState(dotNetExecutable, environment)
        T4DebugProfileWrapperState(wrappee, model, parameters)
      }
      else -> throw CantRunException("Unsupported executor $executorId")
    }
  }
}
