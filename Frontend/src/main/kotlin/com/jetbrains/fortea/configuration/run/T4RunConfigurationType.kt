package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.ConfigurationTypeBase
import com.jetbrains.fortea.icons.T4Icons

object T4RunConfigurationType : ConfigurationTypeBase(
  "RunT4",
  "Run T4 Template",
  "T4 Template Run Configuration",
  T4Icons.T4
)

