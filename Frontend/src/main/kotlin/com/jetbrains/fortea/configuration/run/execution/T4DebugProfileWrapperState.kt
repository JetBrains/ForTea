package com.jetbrains.fortea.configuration.run.execution

import com.intellij.execution.DefaultExecutionResult
import com.intellij.execution.ExecutionResult
import com.intellij.execution.Executor
import com.intellij.execution.runners.ProgramRunner
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.rider.model.T4ProtocolModel
import com.jetbrains.rider.run.IDotNetDebugProfileState

class T4DebugProfileWrapperState(
  private val wrappee: IDotNetDebugProfileState,
  private val model: T4ProtocolModel,
  private val parameters: T4RunConfigurationParameters
) : IDotNetDebugProfileState by wrappee {
  override fun execute(executor: Executor?, runner: ProgramRunner<*>): ExecutionResult? {
    val result: ExecutionResult? = wrappee.execute(executor, runner)
    if (result !is DefaultExecutionResult) return result
    result.processHandler.addProcessListener(T4PostProcessorProcessListener(model, parameters))
    return result
  }
}
