package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.execution.configurations.ConfigurationType
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.openapi.project.Project
import com.jetbrains.rider.run.configurations.dotNetExe.DotNetExeConfiguration
import com.jetbrains.rider.run.configurations.dotNetExe.DotNetExeConfigurationParameters
import org.jetbrains.annotations.NotNull

class T4ConfigurationFactory(type: ConfigurationType) : ConfigurationFactory(type) {
  override fun createTemplateConfiguration(project: Project): RunConfiguration =
    DotNetExeConfiguration("MY CUSTOM NAME", project, this, createParameters(project))

  private fun createParameters(project: Project) = DotNetExeConfigurationParameters(
    project,
    exePath = "",
    programParameters = "",
    workingDirectory = "",
    envs = hashMapOf(),
    isPassParentEnvs = false,
    useExternalConsole = false,
    useMonoRuntime = false,
    executeAsIs = false,
    assemblyToDebug = null,
    runtimeArguments = ""
  )
}
