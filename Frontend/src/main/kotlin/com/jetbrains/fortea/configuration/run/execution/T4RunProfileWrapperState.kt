package com.jetbrains.fortea.configuration.run.execution

import com.intellij.execution.ExecutionResult
import com.intellij.execution.Executor
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.runners.ProgramRunner
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.rider.model.T4ProtocolModel

class T4RunProfileWrapperState(
  private val wrappee: RunProfileState,
  private val model: T4ProtocolModel,
  private val parameters: T4RunConfigurationParameters
) : RunProfileState {
  override fun execute(executor: Executor?, runner: ProgramRunner<*>): ExecutionResult? {
    val result = wrappee.execute(executor, runner)
    model.transferResults.start(parameters.initialFilePath)
    return result
  }
}
