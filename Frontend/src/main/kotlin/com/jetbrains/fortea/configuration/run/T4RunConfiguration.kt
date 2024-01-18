package com.jetbrains.fortea.configuration.run

import com.intellij.execution.Executor
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.project.Project
import com.intellij.openapi.rd.util.withUiContext
import com.jetbrains.fortea.configuration.run.execution.T4ExecutorFactory
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.RiderRunBundle
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

  override suspend fun getRunProfileStateAsync(executor: Executor, environment: ExecutionEnvironment): RunProfileState {
    return withUiContext {
      (executorFactory as T4ExecutorFactory).createAsync(executor.id, environment)
    }
  }

  @Suppress("UsagesOfObsoleteApi")
  @Deprecated("Please, override 'getRunProfileStateAsync' instead")
  override fun getStateAsync(executor: Executor, environment: ExecutionEnvironment): Promise<RunProfileState> {
    @Suppress("DEPRECATION")
    throw UnsupportedOperationException(RiderRunBundle.message("obsolete.synchronous.api.is.used.message", T4RunConfiguration::getStateAsync.name))
  }
}
