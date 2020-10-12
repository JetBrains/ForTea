package com.jetbrains.fortea.configuration.run

import com.jetbrains.fortea.model.T4ExecutionRequest
import com.jetbrains.rider.run.configurations.exe.ExeConfigurationParameters
import com.jetbrains.rider.runtime.DotNetExecutable

open class T4RunConfigurationParameters(
  val request: T4ExecutionRequest,
  exePath: String,
  programParameters: String,
  workingDirectory: String
) : ExeConfigurationParameters(
  exePath,
  programParameters,
  workingDirectory,
  emptyMap(),
  false,
  false
) {
  fun toDotNetExecutable() = DotNetExecutable(
    exePath,
    "",
    workingDirectory,
    programParameters,
    false,
    useExternalConsole,
    envs,
    true,
    { _, _ -> },
    null,
    "",
    false
  )

  override fun validate() {
  }
}
