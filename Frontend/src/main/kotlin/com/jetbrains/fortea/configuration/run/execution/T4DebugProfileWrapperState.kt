package com.jetbrains.fortea.configuration.run.execution

import com.intellij.execution.ExecutionResult
import com.intellij.execution.Executor
import com.intellij.execution.runners.ProgramRunner
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rider.debugger.DebuggerWorkerProcessHandler
import com.jetbrains.fortea.model.T4ProtocolModel
import com.jetbrains.rider.run.IDotNetDebugProfileState

class T4DebugProfileWrapperState(
  private val wrappee: IDotNetDebugProfileState,
  private val model: T4ProtocolModel,
  private val parameters: T4RunConfigurationParameters
) : IDotNetDebugProfileState by wrappee {
  override fun execute(executor: Executor?, runner: ProgramRunner<*>): ExecutionResult? {
    val listener = T4PostProcessorProcessListener(model, parameters)
    try {
      val result = wrappee.execute(executor, runner)
      if (result == null) {
        listener.notifyBackendAboutProcessCompletion(false)
        return null
      }
      result.processHandler.addProcessListener(listener)
      return result
    }
    catch (e: Exception) {
      listener.notifyBackendAboutProcessCompletion(false)
      throw e
    }
  }

  override suspend fun execute(
    executor: Executor,
    runner: ProgramRunner<*>,
    workerProcessHandler: DebuggerWorkerProcessHandler
  ) = listen { wrappee.execute(executor, runner, workerProcessHandler) }

  override suspend fun execute(
    executor: Executor,
    runner: ProgramRunner<*>,
    workerProcessHandler: DebuggerWorkerProcessHandler,
    lifetime: Lifetime
  ) = listen { wrappee.execute(executor, runner, workerProcessHandler, lifetime) }

  private suspend fun listen(startExecution: suspend () -> ExecutionResult): ExecutionResult {
    val listener = T4PostProcessorProcessListener(model, parameters)
    try {
      val result = startExecution()
      result.processHandler.addProcessListener(listener)
      return result
    }
    catch (e: Exception) {
      listener.notifyBackendAboutProcessCompletion(false)
      throw e
    }
  }
}
