package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.execution.configurations.ConfigurationType
import com.intellij.openapi.project.Project
import com.jetbrains.rider.model.T4FileLocation
import org.jetbrains.annotations.NotNull

class T4RunConfigurationFactory(type: ConfigurationType) : ConfigurationFactory(type) {
  // We don't want user to create this configuration manually
  override fun isApplicable(project: Project) = false

  override fun createTemplateConfiguration(@NotNull project: Project) =
    T4RunConfiguration("T4 Template", project, this, createParameters())

  companion object {
    fun createParameters() = T4RunConfigurationParameters(
      exePath = "",
      programParameters = "",
      workingDirectory = "",
      envs = hashMapOf(),
      isPassParentEnvs = false,
      useExternalConsole = false,
      useMonoRuntime = false,
      executeAsIs = false,
      assemblyToDebug = null,
      runtimeArguments = "",
      initialFileLocation = T4FileLocation("", 0)
    )
  }
}
