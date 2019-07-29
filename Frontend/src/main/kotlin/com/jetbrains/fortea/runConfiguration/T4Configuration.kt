package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.runConfiguration.editing.T4SettingsEditorGroup
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.configurations.RiderRunConfiguration
import com.jetbrains.rider.runtime.RiderDotNetActiveRuntimeHost
import com.jetbrains.rider.util.idea.getComponent
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

  private val riderDotNetActiveRuntimeHost = project.getComponent<RiderDotNetActiveRuntimeHost>()

  override fun checkConfiguration() {
    super.checkConfiguration()
    parameters.validate(riderDotNetActiveRuntimeHost)
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
