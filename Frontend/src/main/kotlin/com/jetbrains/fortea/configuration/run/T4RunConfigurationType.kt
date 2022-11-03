package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.ConfigurationTypeBase
import com.jetbrains.fortea.icons.T4Icons
import com.jetbrains.fortea.utils.RiderT4Bundle

object T4RunConfigurationType : ConfigurationTypeBase(
  "RunT4",
  RiderT4Bundle.message("t4.template.run.configuration"),
  RiderT4Bundle.message("run.t4.template"),
  T4Icons.T4
)

