package com.jetbrains.fortea.configuration.execution

import com.jetbrains.fortea.model.T4ExecutionRequest

interface T4RunConfigurationExecutor {
  fun execute(request: T4ExecutionRequest)
}
