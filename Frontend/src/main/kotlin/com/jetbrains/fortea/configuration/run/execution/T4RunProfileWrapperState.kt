package com.jetbrains.fortea.configuration.run.execution

import com.intellij.execution.ExecutionResult
import com.intellij.execution.Executor
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.runners.ProgramRunner
import com.intellij.openapi.diagnostic.thisLogger
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.fortea.model.T4ProtocolModel
import com.jetbrains.rider.run.configurations.RiderAsyncRunProfileState

class T4RunProfileWrapperState(
  private val wrappee: RunProfileState,
  private val model: T4ProtocolModel,
  private val parameters: T4RunConfigurationParameters
) : RunProfileState, RiderAsyncRunProfileState {

  @Deprecated("Use executeAsync instead")
  override fun execute(executor: Executor?, runner: ProgramRunner<*>): ExecutionResult? {
    thisLogger().error("T4RunProfileWrapperState.execute is deprecated. Use executeAsync instead")
    return executeWithListener { wrappee.execute(executor, runner) }
  }

  override suspend fun executeAsync(
    executor: Executor,
    runner: ProgramRunner<*>
  ): ExecutionResult {
    return executeWithListener {
      if(wrappee is RiderAsyncRunProfileState) wrappee.executeAsync(executor, runner)
      else wrappee.execute(executor, runner)
    } ?: throw IllegalStateException("wrappee.execute returned null")
  }

  private  inline fun executeWithListener(executeWrapee: () -> ExecutionResult?) : ExecutionResult? {
    val listener = T4PostProcessorProcessListener(model, parameters)
    try {
      val result: ExecutionResult? = executeWrapee()
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
}
