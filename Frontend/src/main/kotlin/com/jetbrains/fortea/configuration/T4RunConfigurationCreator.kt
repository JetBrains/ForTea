package com.jetbrains.fortea.configuration

import com.intellij.execution.*
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.execution.process.NopProcessHandler
import com.intellij.execution.runners.ExecutionEnvironmentBuilder
import com.intellij.openapi.project.Project
import com.intellij.openapi.vfs.VirtualFile
import com.intellij.util.PathUtil
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
import com.jetbrains.fortea.configuration.run.T4RunConfigurationFactory
import com.jetbrains.fortea.configuration.run.T4RunConfigurationType
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution

class T4RunConfigurationCreator(
  private val project: Project,
  private val host: ProjectModelViewHost
) {
  init {
    project.solution.t4ProtocolModel.requestExecution.set { location ->
      val item = host.getItemById(location.id)
      val file = item?.getVirtualFile() ?: return@set
      launch(DefaultRunExecutor.getRunExecutorInstance(), file)
    }
    project.solution.t4ProtocolModel.requestDebug.set { location ->
      val item = host.getItemById(location.id)
      val file = item?.getVirtualFile() ?: return@set
      launch(DefaultDebugExecutor.getDebugExecutorInstance(), file)
    }
  }

  private fun launch(executor: Executor, file: VirtualFile) {
    val configurationType = ConfigurationTypeUtil.findConfigurationType(T4RunConfigurationType::class.java)
    val configuration = createConfiguration(project, configurationType)
    setupFromFile(configuration, file)
    val runManager = RunManager.getInstance(project)
    val configurationSettings = runManager.createConfiguration(configuration, configurationType.factory)
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

  private fun setupFromFile(configuration: T4RunConfiguration, virtualFile: VirtualFile) {
    val model = configuration.project.solution.t4ProtocolModel
    val t4Path = virtualFile.path
    val host = ProjectModelViewHost.getInstance(configuration.project)
    val item = host.getItemsByVirtualFile(virtualFile).singleOrNull() ?: return
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
      parameters.initialFileLocation = T4FileLocation(item.id)
    }
  }

}
