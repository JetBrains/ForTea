package com.jetbrains.fortea.configuration.run

import com.intellij.execution.configurations.RunProfile
import com.intellij.execution.executors.DefaultRunExecutor
import com.jetbrains.rider.debugger.DotNetProgramRunner

class T4ProgramRunner: DotNetProgramRunner() {
  override fun canRun(executorId: String, runConfiguration: RunProfile): Boolean =
    executorId == DefaultRunExecutor.EXECUTOR_ID && runConfiguration is T4RunConfiguration
}