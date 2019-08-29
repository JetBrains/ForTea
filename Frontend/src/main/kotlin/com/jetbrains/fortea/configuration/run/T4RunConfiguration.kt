package com.jetbrains.fortea.configuration.run

import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.run.execution.T4ExecutorFactory
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.configurations.RiderRunConfiguration

class T4RunConfiguration(
  name: String,
  project: Project,
  val parameters: T4RunConfigurationParameters
) : RiderRunConfiguration(
  name,
  project,
  T4RunConfigurationFactory,
  { throw UnsupportedOperationException() },
  T4ExecutorFactory(project, parameters)
), IRiderDebuggable
