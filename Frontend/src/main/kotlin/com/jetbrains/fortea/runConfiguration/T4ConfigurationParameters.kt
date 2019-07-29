package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.configurations.RuntimeConfigurationError
import com.intellij.openapi.util.JDOMExternalizerUtil
import com.intellij.util.execution.ParametersListUtil
import com.jetbrains.rider.run.configurations.exe.ExeConfigurationParameters
import com.jetbrains.rider.runtime.DotNetExecutable
import com.jetbrains.rider.runtime.RiderDotNetActiveRuntimeHost
import org.jdom.Element

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

  override fun readExternal(element: Element) {
    super.readExternal(element)
    useMonoRuntime = JDOMExternalizerUtil.readField(element, USE_MONO) == "1"
    runtimeArguments = JDOMExternalizerUtil.readField(element, RUNTIME_ARGUMENTS) ?: ""
  }

  override fun writeExternal(element: Element) {
    super.writeExternal(element)
    JDOMExternalizerUtil.writeField(element, USE_MONO, if (useMonoRuntime) "1" else "0")
    JDOMExternalizerUtil.writeField(element, RUNTIME_ARGUMENTS, runtimeArguments)
  }

  companion object {
    private const val USE_MONO = "USE_MONO"
    private const val RUNTIME_ARGUMENTS = "RUNTIME_ARGUMENTS"
  }
}
