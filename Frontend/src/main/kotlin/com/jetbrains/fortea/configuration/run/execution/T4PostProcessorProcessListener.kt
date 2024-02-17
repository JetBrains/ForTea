package com.jetbrains.fortea.configuration.run.execution

import com.intellij.execution.process.ProcessEvent
import com.intellij.execution.process.ProcessListener
import com.intellij.openapi.util.Key
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.fortea.utils.handleEndOfExecution
import com.jetbrains.fortea.model.T4ProtocolModel

class T4PostProcessorProcessListener(
  private val model: T4ProtocolModel,
  private val parameters: T4RunConfigurationParameters
) : ProcessListener {
  override fun onTextAvailable(p0: ProcessEvent, p1: Key<*>) {
  }

  override fun processTerminated(p0: ProcessEvent) = notifyBackendAboutProcessCompletion(p0.exitCode == 0)

  fun notifyBackendAboutProcessCompletion(success: Boolean) {
    val call = if (success) model.executionSucceeded else model.executionFailed
    call.handleEndOfExecution(parameters.request.location)
  }

  override fun processWillTerminate(p0: ProcessEvent, p1: Boolean) {
  }

  override fun startNotified(p0: ProcessEvent) {
  }
}
