package com.jetbrains.fortea.configuration.run

import com.jetbrains.fortea.model.T4ExecutionRequest
import com.jetbrains.rider.run.configurations.exe.ExeConfigurationParameters
import com.jetbrains.rider.runtime.DotNetExecutable

open class T4RunConfigurationParameters(
  val request: T4ExecutionRequest,
  exePath: String,
  programParameters: String,
  private val envDTEPort: Int,
  workingDirectory: String
) : ExeConfigurationParameters(
  exePath,
  programParameters,
  workingDirectory,
  emptyMap(),
  false,
) {
  override fun createDotNetExecutableTemplate(): DotNetExecutable = DotNetExecutable(
    exePath,
    null,
    workingDirectory,
    programParameters,
    terminalMode,
    envs + ("T4_ENVDTE_CLIENT_PORT" to envDTEPort.toString()),
    true,
    { _, _, _ -> },
    null,
    "",
    false,
    mixedModeDebugging = false
  )

  override fun validate() {
  }
}
