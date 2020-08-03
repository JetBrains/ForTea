package com.jetbrains.fortea.configuration.execution.impl

import com.intellij.execution.RunManager
import com.intellij.execution.RunnerAndConfigurationSettings
import com.intellij.openapi.project.Project
import com.intellij.util.PathUtil
import com.jetbrains.fortea.configuration.execution.T4RunConfigurationExecutor
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
import com.jetbrains.fortea.configuration.run.T4RunConfigurationFactory
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.rider.model.T4ExecutionRequest
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution

abstract class T4RunConfigurationExecutorBase(
  protected val project: Project,
  private val host: ProjectModelViewHost
) : T4RunConfigurationExecutor {
  final override fun execute(request: T4ExecutionRequest) {
    val configuration = setupFromFile(request)
    val runManager = RunManager.getInstance(project)
    val configurationSettings = runManager.createConfiguration(configuration, T4RunConfigurationFactory)
    configurationSettings.isActivateToolWindowBeforeRun = request.isVisible
    executeConfiguration(configurationSettings)
  }

  private fun setupFromFile(request: T4ExecutionRequest): T4RunConfiguration {
    val model = project.solution.t4ProtocolModel
    val item = host.getItemById(request.location.id)
    val virtualFile = item?.getVirtualFile()!!
    val t4Path = virtualFile.path
    val protocolConfiguration = model.getConfiguration.sync(T4FileLocation(item.id))
    val parameters = T4RunConfigurationParameters(
      request,
      protocolConfiguration.executablePath,
      "\"${protocolConfiguration.outputPath}\"",
      protocolConfiguration.envDTEPort,
      PathUtil.getParentPath(t4Path)
    )
    return T4RunConfiguration(virtualFile.name, project, parameters)
  }

  protected abstract fun executeConfiguration(configuration: RunnerAndConfigurationSettings)
}
