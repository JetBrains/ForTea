package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.RunProfile
import com.intellij.execution.executors.DefaultDebugExecutor
import com.jetbrains.rider.debugger.DotNetDebugRunner

class T4DebugRunner: DotNetDebugRunner() {
  override fun canRun(executorId: String, runConfiguration: RunProfile): Boolean =
    executorId == DefaultDebugExecutor.EXECUTOR_ID && runConfiguration is T4RunConfiguration
}