package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.configurations.RuntimeConfigurationError
import com.intellij.util.execution.ParametersListUtil
import com.jetbrains.rider.run.configurations.exe.ExeConfigurationParameters
import com.jetbrains.rider.runtime.DotNetExecutable
import com.jetbrains.rider.runtime.RiderDotNetActiveRuntimeHost

open class T4ConfigurationParameters(
  exePath: String,
  programParameters: String,
  workingDirectory: String,
  envs: Map<String, String>,
  isPassParentEnvs: Boolean,
  useExternalConsole: Boolean,
  var useMonoRuntime: Boolean,
  private var executeAsIs: Boolean,
  private val assemblyToDebug: String?,
  var runtimeArguments: String
) : ExeConfigurationParameters(
  exePath,
  programParameters,
  workingDirectory,
  envs,
  isPassParentEnvs,
  useExternalConsole
) {
  open fun toDotNetExecutable() = DotNetExecutable(
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

  open fun validate(riderDotNetActiveRuntimeHost: RiderDotNetActiveRuntimeHost) {
    super.validate()

    if (useMonoRuntime && riderDotNetActiveRuntimeHost.monoRuntime == null)
      throw RuntimeConfigurationError("Mono runtime not found. Please setup Mono path in settings (File | Settings | Build, Execution, Deployment | Toolset and Build)")
  }
}
