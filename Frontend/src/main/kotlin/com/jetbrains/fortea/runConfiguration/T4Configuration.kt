package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.runConfiguration.editing.T4SettingsEditorGroup
import com.jetbrains.fortea.runConfiguration.execution.T4ExecutorFactory
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.configurations.RiderRunConfiguration
import org.jdom.Element

class T4Configuration(
  name: String,
  project: Project,
  factory: ConfigurationFactory,
  val parameters: T4ConfigurationParameters
) : RiderRunConfiguration(
  name, project, factory, { T4SettingsEditorGroup(it) },
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
