package com.jetbrains.fortea.configuration.preprocess

import com.intellij.execution.actions.ConfigurationContext
import com.intellij.execution.actions.RunConfigurationProducer
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.openapi.util.Ref
import com.intellij.psi.PsiElement
import com.jetbrains.fortea.psi.T4PsiFile

class T4PreprocessConfigurationProducer : RunConfigurationProducer<T4PreprocessConfiguration>(
  ConfigurationTypeUtil.findConfigurationType(T4PreprocessConfigurationType::class.java)
) {
  override fun isConfigurationFromContext(
    configuration: T4PreprocessConfiguration,
    context: ConfigurationContext
  ) = true

  override fun setupConfigurationFromContext(
    configuration: T4PreprocessConfiguration,
    context: ConfigurationContext,
    sourceElement: Ref<PsiElement>
  ): Boolean {
    val t4File = sourceElement.get().containingFile as? T4PsiFile ?: return false
    val t4Path = t4File.virtualFile.path

    configuration.name = "Preprocess " + t4File.name
    configuration.parameters.initialFilePath = t4Path

    return true
  }
}
