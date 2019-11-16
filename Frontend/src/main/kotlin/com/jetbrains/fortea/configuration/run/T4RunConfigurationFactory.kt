package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.openapi.project.Project
import com.jetbrains.rider.model.T4ExecutionRequest
import com.jetbrains.rider.model.T4FileLocation
import org.jetbrains.annotations.NotNull

object T4RunConfigurationFactory : ConfigurationFactory(T4RunConfigurationType) {
  override fun createTemplateConfiguration(@NotNull project: Project) = T4RunConfiguration(
    "T4 Template", project, T4RunConfigurationParameters(T4ExecutionRequest(T4FileLocation(0), false), "", "", "")
  )
}
