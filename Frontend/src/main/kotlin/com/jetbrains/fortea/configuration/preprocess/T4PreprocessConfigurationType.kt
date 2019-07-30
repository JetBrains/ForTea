package com.jetbrains.fortea.configuration.preprocess

import com.intellij.execution.configurations.ConfigurationTypeBase
import javax.swing.Icon

class T4PreprocessConfigurationType : ConfigurationTypeBase(
  "PreprocessT4",
  "Preprocess T4 Template",
  "T4 Template Preprocessing Configuration",
  null as Icon?
) {
  private val factory = T4PreprocessConfigurationFactory(this)
  override fun getHelpTopic() = "reference.dialogs.rundebug.PreprocessT4"

  init {
    addFactory(factory)
  }
}
