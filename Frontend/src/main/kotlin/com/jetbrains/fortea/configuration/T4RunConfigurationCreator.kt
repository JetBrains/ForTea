package com.jetbrains.fortea.configuration

import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.execution.impl.T4AsyncDebugConfigurationExecutor
import com.jetbrains.fortea.configuration.execution.impl.T4AsyncRunConfigurationExecutor
import com.jetbrains.fortea.model.t4ProtocolModel
import com.jetbrains.rider.projectView.solution

class T4RunConfigurationCreator(project: Project) {
  init {
    val model = project.solution.t4ProtocolModel
    model.requestExecution.set(handler = T4AsyncRunConfigurationExecutor(project)::execute)
    model.requestDebug.set(handler = T4AsyncDebugConfigurationExecutor(project)::execute)
  }
}
