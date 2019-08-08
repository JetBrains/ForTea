package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.ConfigurationTypeBase
import com.jetbrains.fortea.configuration.T4ConfigurationType
import com.jetbrains.fortea.icons.T4Icons

class T4RunConfigurationType : ConfigurationTypeBase(
  "RunT4",
  "Run T4 Template",
  "T4 Template Run Configuration",
  T4Icons.T4
), T4ConfigurationType {
  override val factory = T4RunConfigurationFactory(this)
  override fun getHelpTopic() = "reference.dialogs.rundebug.RunT4"

  init {
    addFactory(factory)
  }
}

