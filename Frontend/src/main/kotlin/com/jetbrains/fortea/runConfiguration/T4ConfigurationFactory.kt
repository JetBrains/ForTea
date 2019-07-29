package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.BeforeRunTask
import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.execution.configurations.ConfigurationType
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.Key
import org.jetbrains.annotations.NotNull

class T4ConfigurationFactory(type: ConfigurationType) : ConfigurationFactory(type) {
  override fun configureBeforeRunTaskDefaults(
    providerID: Key<out BeforeRunTask<BeforeRunTask<*>>>?,
    task: BeforeRunTask<out BeforeRunTask<*>>?
  ) = Unit

  override fun isConfigurationSingletonByDefault() = true

  // We don't want user to create this configuration manually
  override fun isApplicable(project: Project) = false

  override fun createConfiguration(name: String?, template: RunConfiguration): RunConfiguration =
    T4Configuration(name ?: "T4 Template", template.project, this, createParameters())

  override fun createTemplateConfiguration(@NotNull project: Project): RunConfiguration =
    T4Configuration("T4 Template", project, this, createParameters())

  private fun createParameters() = T4ConfigurationParameters(
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
