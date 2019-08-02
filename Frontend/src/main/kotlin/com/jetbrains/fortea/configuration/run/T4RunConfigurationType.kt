package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.ConfigurationTypeBase
import javax.swing.Icon

class T4RunConfigurationType : ConfigurationTypeBase(
  "RunT4",
  "Run T4 Template",
  "T4 Template Run Configuration",
  null as Icon?
) {

  val factory = T4RunConfigurationFactory(this)
  override fun getHelpTopic() = "reference.dialogs.rundebug.RunT4"

  init {
    addFactory(factory)
  }
}

