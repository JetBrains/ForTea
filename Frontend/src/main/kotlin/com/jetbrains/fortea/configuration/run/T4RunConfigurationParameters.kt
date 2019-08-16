package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.RuntimeConfigurationError
import com.intellij.openapi.util.JDOMExternalizerUtil
import com.intellij.util.execution.ParametersListUtil
import com.jetbrains.fortea.configuration.readT4FileLocationExternal
import com.jetbrains.fortea.configuration.writeExternal
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.run.configurations.exe.ExeConfigurationParameters
import com.jetbrains.rider.runtime.DotNetExecutable
import org.jdom.Element
import java.io.File

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
  var initialFileLocation: T4FileLocation
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

  override fun validate() {
    if (File(initialFileLocation.location).exists()) return
    throw RuntimeConfigurationError("Target file does not exist")
  }

  override fun readExternal(element: Element) {
    super.readExternal(element)
    useMonoRuntime = JDOMExternalizerUtil.readField(element, USE_MONO) == "1"
    runtimeArguments = JDOMExternalizerUtil.readField(element, RUNTIME_ARGUMENTS) ?: ""
    initialFileLocation = readT4FileLocationExternal(element)
  }

  override fun writeExternal(element: Element) {
    super.writeExternal(element)
    JDOMExternalizerUtil.writeField(element, USE_MONO, if (useMonoRuntime) "1" else "0")
    JDOMExternalizerUtil.writeField(element, RUNTIME_ARGUMENTS, runtimeArguments)
    initialFileLocation.writeExternal(element)
  }

  companion object {
    private const val USE_MONO = "USE_MONO"
    private const val RUNTIME_ARGUMENTS = "RUNTIME_ARGUMENTS"
  }
}
