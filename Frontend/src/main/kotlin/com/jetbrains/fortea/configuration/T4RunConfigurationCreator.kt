package com.jetbrains.fortea.configuration

import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.execution.impl.T4AsyncDebugConfigurationExecutor
import com.jetbrains.fortea.configuration.execution.impl.T4AsyncRunConfigurationExecutor
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution

class T4RunConfigurationCreator(project: Project, host: ProjectModelViewHost) {
  init {
    val model = project.solution.t4ProtocolModel
    model.requestExecution.set(T4AsyncRunConfigurationExecutor(project, host)::execute)
    model.requestDebug.set(T4AsyncDebugConfigurationExecutor(project, host)::execute)
  }
}
