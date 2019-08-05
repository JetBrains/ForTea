package com.jetbrains.fortea.configuration.actions

import com.intellij.execution.Executor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfiguration
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationParameters
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationType
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.icons.ReSharperAssemblyExplorerIcons

class T4PreprocessTemplateAction : T4FileBasedActionBase<T4PreprocessConfiguration, T4PreprocessConfigurationType>(
  "Preprocess T4 Template",
  ReSharperAssemblyExplorerIcons.AddProcess
) {
  override fun createConfiguration(
    project: Project,
    configurationType: T4PreprocessConfigurationType
  ) = T4PreprocessConfiguration("", project, configurationType.factory, T4PreprocessConfigurationParameters(""))

  override fun setupFromFile(configuration: T4PreprocessConfiguration, file: T4PsiFile) {
    val t4Path = file.virtualFile.path
    configuration.name = "Preprocess " + file.name
    configuration.parameters.initialFilePath = t4Path
  }

  override val configurationTypeClass = T4PreprocessConfigurationType::class
  override val executor: Executor = DefaultRunExecutor.getRunExecutorInstance()
}
