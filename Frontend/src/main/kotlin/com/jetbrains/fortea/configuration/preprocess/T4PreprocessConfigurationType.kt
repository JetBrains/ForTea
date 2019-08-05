package com.jetbrains.fortea.configuration.preprocess

import com.intellij.execution.configurations.ConfigurationTypeBase
import com.jetbrains.fortea.configuration.T4ConfigurationType
import javax.swing.Icon

class T4PreprocessConfigurationType : ConfigurationTypeBase(
  "PreprocessT4",
  "Preprocess T4 Template",
  "T4 Template Preprocessing Configuration",
  null as Icon?
), T4ConfigurationType {
  override val factory = T4PreprocessConfigurationFactory(this)
  override fun getHelpTopic() = "reference.dialogs.rundebug.PreprocessT4"

  init {
    addFactory(factory)
  }
}
