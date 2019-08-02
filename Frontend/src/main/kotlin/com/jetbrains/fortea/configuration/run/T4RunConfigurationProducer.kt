package com.jetbrains.fortea.configuration.run

import com.intellij.execution.actions.ConfigurationContext
import com.intellij.execution.actions.RunConfigurationProducer
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.openapi.util.Ref
import com.intellij.psi.PsiElement
import com.intellij.util.PathUtil
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.solution

class T4RunConfigurationProducer : RunConfigurationProducer<T4RunConfiguration>(
  ConfigurationTypeUtil.findConfigurationType(T4RunConfigurationType::class.java)
) {
  override fun isConfigurationFromContext(configuration: T4RunConfiguration, context: ConfigurationContext): Boolean {
    val parametersPath = configuration.parameters.initialFilePath
    val t4File = context.psiLocation?.containingFile as? T4PsiFile ?: return false
    val t4Path = t4File.virtualFile.path
    return t4Path == parametersPath
  }

  override fun setupConfigurationFromContext(
    configuration: T4RunConfiguration,
    context: ConfigurationContext,
    sourceElement: Ref<PsiElement>
  ): Boolean {
    val t4File = sourceElement.get().containingFile as? T4PsiFile ?: return false
    return setupFromFile(configuration, t4File)
  }

  companion object {
    fun setupFromFile(
      configuration: T4RunConfiguration,
      t4File: T4PsiFile
    ): Boolean {
      val model = configuration.project.solution.t4ProtocolModel
      val t4Path = t4File.virtualFile.path
      val protocolConfiguration = model.configurations[t4Path] ?: return false

      configuration.name = t4File.name
      configuration.parameters.exePath = protocolConfiguration.executablePath
      configuration.parameters.programParameters = protocolConfiguration.outputPath
      configuration.parameters.isPassParentEnvs = false
      configuration.parameters.runtimeArguments = ""
      configuration.parameters.useMonoRuntime = false
      configuration.parameters.envs = emptyMap()
      configuration.parameters.workingDirectory = PathUtil.getParentPath(t4Path)
      configuration.parameters.initialFilePath = t4Path

      return true
    }

    fun canSetup(t4File: T4PsiFile): Boolean =
      t4File.project.solution.t4ProtocolModel.configurations.containsKey(t4File.virtualFile.path)
  }
}
