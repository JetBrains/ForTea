package com.jetbrains.fortea.configuration.run.execution

import com.intellij.execution.DefaultExecutionResult
import com.intellij.execution.Executor
import com.intellij.execution.runners.ProgramRunner
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rider.debugger.DebuggerWorkerProcessHandler
import com.jetbrains.rider.model.T4ProtocolModel
import com.jetbrains.rider.run.IDotNetDebugProfileState

class T4DebugProfileWrapperState(
  private val wrappee: IDotNetDebugProfileState,
  private val model: T4ProtocolModel,
  private val parameters: T4RunConfigurationParameters
) : IDotNetDebugProfileState by wrappee {
  override fun execute(executor: Executor?, runner: ProgramRunner<*>) =
    listen(wrappee.execute(executor, runner) as DefaultExecutionResult)

  override fun execute(
    executor: Executor,
    runner: ProgramRunner<*>,
    workerProcessHandler: DebuggerWorkerProcessHandler
  ) = listen(wrappee.execute(executor, runner, workerProcessHandler) as DefaultExecutionResult)

  override fun execute(
    executor: Executor,
    runner: ProgramRunner<*>,
    workerProcessHandler: DebuggerWorkerProcessHandler,
    lifetime: Lifetime
  ) = listen(wrappee.execute(executor, runner, workerProcessHandler, lifetime) as DefaultExecutionResult)

  private fun listen(result: DefaultExecutionResult): DefaultExecutionResult {
    result.processHandler.addProcessListener(T4PostProcessorProcessListener(model, parameters))
    return result
  }
}
