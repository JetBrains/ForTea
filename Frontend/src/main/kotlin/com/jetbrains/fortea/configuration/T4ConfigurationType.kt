package com.jetbrains.fortea.configuration

import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.execution.configurations.ConfigurationType

interface T4ConfigurationType : ConfigurationType {
  val factory: ConfigurationFactory
}
