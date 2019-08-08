package com.jetbrains.fortea.configuration.preprocess

import com.intellij.execution.configurations.ConfigurationTypeBase
import com.jetbrains.fortea.configuration.T4ConfigurationType
import com.jetbrains.rider.icons.ReSharperLiveTemplatesCSharpIcons

class T4PreprocessConfigurationType : ConfigurationTypeBase(
  "PreprocessT4",
  "Preprocess T4 Template",
  "T4 Template Preprocessing Configuration",
  ReSharperLiveTemplatesCSharpIcons.ScopeCS
), T4ConfigurationType {
  override val factory = T4PreprocessConfigurationFactory(this)
  override fun getHelpTopic() = "reference.dialogs.rundebug.PreprocessT4"

  init {
    addFactory(factory)
  }
}
