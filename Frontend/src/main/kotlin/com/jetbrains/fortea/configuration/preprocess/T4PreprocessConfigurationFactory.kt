package com.jetbrains.fortea.configuration.preprocess

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.execution.configurations.ConfigurationType
import com.intellij.openapi.project.Project
import com.jetbrains.rider.model.T4FileLocation

class T4PreprocessConfigurationFactory(type: ConfigurationType) : ConfigurationFactory(type) {
  // We don't want user to create this configuration manually
  override fun isApplicable(project: Project) = false

  override fun createTemplateConfiguration(project: Project) = T4PreprocessConfiguration(
    "Preprocess T4 Template",
    project,
    this,
    T4PreprocessConfigurationParameters(T4FileLocation("", 0))
  )
}
