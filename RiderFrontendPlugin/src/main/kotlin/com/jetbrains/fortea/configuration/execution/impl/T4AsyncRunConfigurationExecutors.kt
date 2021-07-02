package com.jetbrains.fortea.configuration.execution.impl

import com.intellij.execution.Executor
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.openapi.project.Project

class T4AsyncDebugConfigurationExecutor(project: Project) : T4AsyncRunConfigurationExecutorBase(project) {
  override val executor: Executor = DefaultDebugExecutor.getDebugExecutorInstance()
}

class T4AsyncRunConfigurationExecutor(project: Project) : T4AsyncRunConfigurationExecutorBase(project) {
  override val executor: Executor = DefaultRunExecutor.getRunExecutorInstance()
}
