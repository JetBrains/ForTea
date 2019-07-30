package com.jetbrains.fortea.configuration.preprocess.execution

import com.intellij.execution.DefaultExecutionResult
import com.intellij.execution.ExecutionResult
import com.intellij.execution.Executor
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.runners.ProgramRunner
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationParameters
import com.jetbrains.rider.model.T4ProtocolModel

class T4PreprocessRunProfileState(
  private val parameters: T4PreprocessConfigurationParameters,
  private val model: T4ProtocolModel
) : RunProfileState {
  override fun execute(executor: Executor?, runner: ProgramRunner<*>): ExecutionResult? {
    model.requestPreprocessing.start(parameters.initialFilePath)
    return DefaultExecutionResult()
  }
}
