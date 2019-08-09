package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.run.editing.T4RunConfigurationSettingsEditorGroup
import com.jetbrains.fortea.configuration.run.execution.T4ExecutorFactory
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.configurations.RiderRunConfiguration
import org.jdom.Element

class T4RunConfiguration(
  name: String,
  project: Project,
  factory: ConfigurationFactory,
  val parameters: T4RunConfigurationParameters
) : RiderRunConfiguration(
  name, project, factory, { T4RunConfigurationSettingsEditorGroup(it) },
  T4ExecutorFactory(project, parameters)
), IRiderDebuggable {

  override fun checkConfiguration() {
    super.checkConfiguration()
    parameters.validate()
  }

  override fun readExternal(element: Element) {
    super.readExternal(element)
    parameters.readExternal(element)
  }

  override fun writeExternal(element: Element) {
    super.writeExternal(element)
    parameters.writeExternal(element)
  }
}
