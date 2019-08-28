package com.jetbrains.fortea.configuration.run

import com.intellij.util.execution.ParametersListUtil
import com.jetbrains.rider.model.T4ExecutionRequest
import com.jetbrains.rider.run.configurations.exe.ExeConfigurationParameters
import com.jetbrains.rider.runtime.DotNetExecutable

open class T4RunConfigurationParameters(
  exePath: String,
  programParameters: String,
  workingDirectory: String,
  envs: Map<String, String>,
  isPassParentEnvs: Boolean,
  useExternalConsole: Boolean,
  var useMonoRuntime: Boolean,
  private var executeAsIs: Boolean,
  private val assemblyToDebug: String?,
  var runtimeArguments: String,
  var request: T4ExecutionRequest
) : ExeConfigurationParameters(
  exePath,
  programParameters,
  workingDirectory,
  envs,
  isPassParentEnvs,
  useExternalConsole
) {
  fun toDotNetExecutable() = DotNetExecutable(
    exePath,
    "",
    workingDirectory,
    ParametersListUtil.parse(programParameters),
    useMonoRuntime,
    useExternalConsole,
    envs,
    isPassParentEnvs,
    { _, _ -> },
    assemblyToDebug,
    runtimeArguments,
    executeAsIs
  )

  override fun validate() {
  }
}
