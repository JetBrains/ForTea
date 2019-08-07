package com.jetbrains.fortea.configuration.preprocess.execution

import com.intellij.execution.CantRunException
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.execution.runners.ExecutionEnvironment
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationParameters
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.run.configurations.IExecutorFactory

class T4PreprocessorFactory(private val parameters: T4PreprocessConfigurationParameters) : IExecutorFactory {
  override fun create(executorId: String, environment: ExecutionEnvironment) = when (executorId) {
    DefaultRunExecutor.EXECUTOR_ID ->
      T4PreprocessRunProfileState(parameters, environment.project.solution.t4ProtocolModel, environment.project)
    else -> throw CantRunException("Unsupported executor $executorId")
  }
}
