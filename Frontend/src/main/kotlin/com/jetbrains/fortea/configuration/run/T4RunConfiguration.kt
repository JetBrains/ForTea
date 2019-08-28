package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.run.editing.T4RunConfigurationSettingsEditorGroup
import com.jetbrains.fortea.configuration.run.execution.T4ExecutorFactory
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.configurations.RiderRunConfiguration

class T4RunConfiguration(
  name: String,
  project: Project,
  factory: ConfigurationFactory,
  val parameters: T4RunConfigurationParameters
) : RiderRunConfiguration(
  name,
  project,
  factory,
  { T4RunConfigurationSettingsEditorGroup(it) },
  T4ExecutorFactory(project, parameters)
), IRiderDebuggable
