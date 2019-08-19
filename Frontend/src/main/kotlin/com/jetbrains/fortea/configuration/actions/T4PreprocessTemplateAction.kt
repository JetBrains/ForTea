package com.jetbrains.fortea.configuration.actions

import com.intellij.execution.Executor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfiguration
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationParameters
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationType
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.icons.ReSharperLiveTemplatesCSharpIcons
import com.jetbrains.rider.model.T4FileLocation

class T4PreprocessTemplateAction : T4FileBasedActionBase<T4PreprocessConfiguration, T4PreprocessConfigurationType>(
  "Generate code",
  ReSharperLiveTemplatesCSharpIcons.ScopeCS
) {
  override fun createConfiguration(
    project: Project,
    configurationType: T4PreprocessConfigurationType
  ) = T4PreprocessConfiguration(
    "",
    project,
    configurationType.factory,
    T4PreprocessConfigurationParameters(T4FileLocation("", 0))
  )

  override fun setupFromFile(
    configuration: T4PreprocessConfiguration,
    file: T4PsiFile,
    projectId: Int
  ) {
    val t4Path = file.virtualFile.path
    configuration.name = "Preprocess " + file.name
    configuration.parameters.initialFileLocation = T4FileLocation(t4Path, projectId)
  }

  override val configurationTypeClass = T4PreprocessConfigurationType::class
  override val executor: Executor = DefaultRunExecutor.getRunExecutorInstance()
}
