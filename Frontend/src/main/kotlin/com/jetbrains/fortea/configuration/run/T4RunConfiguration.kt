package com.jetbrains.fortea.configuration.run

import com.intellij.execution.Executor
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.run.execution.T4ExecutorFactory
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.configurations.AsyncRunConfiguration
import com.jetbrains.rider.run.configurations.RiderRunConfiguration
import com.jetbrains.rider.run.configurations.dotNetExe.DotNetExeExecutorFactory
import org.jetbrains.concurrency.Promise

class T4RunConfiguration(
  name: String,
  project: Project,
  val parameters: T4RunConfigurationParameters
) : RiderRunConfiguration(
  name,
  project,
  T4RunConfigurationFactory,
  { throw UnsupportedOperationException() },
  T4ExecutorFactory(project, parameters)
), IRiderDebuggable, AsyncRunConfiguration
{
  override fun getStateAsync(executor: Executor, environment: ExecutionEnvironment): Promise<RunProfileState> {
    val factory = executorFactory as T4ExecutorFactory
    return factory.createAsync(executor.id, environment)
  }
}