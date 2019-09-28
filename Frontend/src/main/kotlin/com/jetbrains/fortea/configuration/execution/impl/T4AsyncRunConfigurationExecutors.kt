package com.jetbrains.fortea.configuration.execution.impl

import com.intellij.execution.Executor
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.openapi.project.Project
import com.jetbrains.rider.projectView.ProjectModelViewHost

class T4AsyncDebugConfigurationExecutor(
  project: Project,
  host: ProjectModelViewHost
) : T4AsyncRunConfigurationExecutorBase(project, host) {
  override val executor: Executor = DefaultDebugExecutor.getDebugExecutorInstance()
}

class T4AsyncRunConfigurationExecutor(
  project: Project,
  host: ProjectModelViewHost
) : T4AsyncRunConfigurationExecutorBase(project, host) {
  override val executor: Executor = DefaultRunExecutor.getRunExecutorInstance()
}
