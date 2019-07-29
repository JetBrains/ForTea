package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.actions.ConfigurationContext
import com.intellij.execution.actions.RunConfigurationProducer
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.openapi.util.Ref
import com.intellij.psi.PsiElement
import com.intellij.util.PathUtil
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.solution

class T4RunConfigurationProducer : RunConfigurationProducer<T4Configuration>(
  ConfigurationTypeUtil.findConfigurationType(T4ConfigurationType::class.java)
) {
  override fun isConfigurationFromContext(configuration: T4Configuration, context: ConfigurationContext) = true

  override fun setupConfigurationFromContext(
    configuration: T4Configuration,
    context: ConfigurationContext,
    sourceElement: Ref<PsiElement>
  ): Boolean {
    val t4File = sourceElement.get().containingFile as? T4PsiFile ?: return false
    val model = configuration.project.solution.t4ProtocolModel
    val path = t4File.virtualFile.path
    val protocolConfiguration = model.configurations[path] ?: return false

    configuration.name = t4File.name
    configuration.parameters.exePath = protocolConfiguration.executablePath
    configuration.parameters.programParameters = protocolConfiguration.outputName
    configuration.parameters.isPassParentEnvs = false
    configuration.parameters.runtimeArguments = ""
    configuration.parameters.useMonoRuntime = false
    configuration.parameters.envs = emptyMap()
    configuration.parameters.workingDirectory = PathUtil.getParentPath(path)
    configuration.beforeRunTasks = emptyList()

    return true
  }
}
