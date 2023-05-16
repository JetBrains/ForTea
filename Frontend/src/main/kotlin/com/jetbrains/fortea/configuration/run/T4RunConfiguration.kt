package com.jetbrains.fortea.configuration.run

import com.intellij.execution.Executor
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.project.Project
import com.intellij.openapi.rd.util.startOnUiAsync
import com.jetbrains.fortea.configuration.run.execution.T4ExecutorFactory
import com.jetbrains.rd.platform.util.lifetime
import com.jetbrains.rd.platform.util.toPromise
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.configurations.AsyncRunConfiguration
import com.jetbrains.rider.run.configurations.RiderRunConfiguration
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
), IRiderDebuggable, AsyncRunConfiguration {
  override fun getStateAsync(executor: Executor, environment: ExecutionEnvironment): Promise<RunProfileState> {
    return environment.project.lifetime.startOnUiAsync {
      (executorFactory as T4ExecutorFactory).createAsync(executor.id, environment)
    }.toPromise()
  }
}
