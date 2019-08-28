package com.jetbrains.fortea.configuration

import com.intellij.execution.*
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.execution.process.NopProcessHandler
import com.intellij.execution.runners.ExecutionEnvironmentBuilder
import com.intellij.openapi.project.Project
import com.intellij.util.PathUtil
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
import com.jetbrains.fortea.configuration.run.T4RunConfigurationFactory
import com.jetbrains.fortea.configuration.run.T4RunConfigurationType
import com.jetbrains.rider.model.T4ExecutionRequest
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution

class T4RunConfigurationCreator(
  private val project: Project,
  private val host: ProjectModelViewHost
) {
  init {
    val model = project.solution.t4ProtocolModel
    model.requestExecution.set(launcher(DefaultRunExecutor.getRunExecutorInstance()))
    model.requestDebug.set(launcher(DefaultDebugExecutor.getDebugExecutorInstance()))
  }

  private fun launcher(executor: Executor) = { request : T4ExecutionRequest ->
    val configurationType = ConfigurationTypeUtil.findConfigurationType(T4RunConfigurationType::class.java)
    val configuration = createConfiguration(project, configurationType)
    setupFromFile(configuration, request)
    val runManager = RunManager.getInstance(project)
    val configurationSettings = runManager.createConfiguration(configuration, configurationType.factory)
    configurationSettings.isActivateToolWindowBeforeRun = request.isVisible
    executeConfiguration(configurationSettings, executor, project)
  }

  private fun executeConfiguration(
    configuration: RunnerAndConfigurationSettings,
    executor: Executor,
    project: Project
  ) {
    val builder: ExecutionEnvironmentBuilder
    try {
      builder = ExecutionEnvironmentBuilder.create(executor, configuration)
    } catch (e: ExecutionException) {
      return
    }

    val environment = builder.contentToReuse(null).dataContext(null).activeTarget().build()
    ProgramRunnerUtil.executeConfigurationAsync(
      environment,
      true,
      true
    ) {
      val listener = project.messageBus.syncPublisher(ExecutionManager.EXECUTION_TOPIC)
      listener.processStarted(executor.id, environment, NopProcessHandler())
    }
  }

  private fun createConfiguration(project: Project, configurationType: T4RunConfigurationType) =
    T4RunConfiguration("", project, configurationType.factory, T4RunConfigurationFactory.createParameters())

  private fun setupFromFile(configuration: T4RunConfiguration, request: T4ExecutionRequest) {
    val model = configuration.project.solution.t4ProtocolModel
    val item = host.getItemById(request.location.id)
    val virtualFile = item?.getVirtualFile() ?: return
    val t4Path = virtualFile.path
    val protocolConfiguration = model.getConfiguration.sync(T4FileLocation(item.id))
    with(configuration) {
      name = virtualFile.name
      parameters.exePath = protocolConfiguration.executablePath
      parameters.programParameters = protocolConfiguration.outputPath
      parameters.isPassParentEnvs = false
      parameters.runtimeArguments = ""
      parameters.useMonoRuntime = false
      parameters.envs = emptyMap()
      parameters.workingDirectory = PathUtil.getParentPath(t4Path)
      parameters.request = request
    }
  }

}
