package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.configurations.ConfigurationTypeBase
import javax.swing.Icon

class T4ConfigurationType : ConfigurationTypeBase(
  "RunT4",
  "T4 Template",
  "T4 Template Run Configuration",
  null as Icon?
) {

  private val factory = T4ConfigurationFactory(this)
  override fun getHelpTopic() = "reference.dialogs.rundebug.RunT4"

  init {
    addFactory(factory)
  }
}

