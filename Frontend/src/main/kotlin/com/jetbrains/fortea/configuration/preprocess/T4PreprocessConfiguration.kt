package com.jetbrains.fortea.configuration.preprocess

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.preprocess.editing.T4PreprocessConfigurationSettingsEditorGroup
import com.jetbrains.fortea.configuration.preprocess.execution.T4PreprocessorFactory
import com.jetbrains.rider.debugger.IRiderDebuggable
import com.jetbrains.rider.run.configurations.RiderRunConfiguration
import org.jdom.Element

class T4PreprocessConfiguration(
  name: String,
  project: Project,
  factory: ConfigurationFactory,
  val parameters: T4PreprocessConfigurationParameters
) : RiderRunConfiguration(
  name,
  project,
  factory,
  { T4PreprocessConfigurationSettingsEditorGroup(it) },
  T4PreprocessorFactory(parameters)
), IRiderDebuggable {
  override fun isNative() = true

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
