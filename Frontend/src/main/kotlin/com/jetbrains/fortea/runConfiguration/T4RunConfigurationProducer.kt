package com.jetbrains.fortea.runConfiguration

import com.intellij.execution.RunManager
import com.intellij.execution.RunnerAndConfigurationSettings
import com.intellij.execution.actions.ConfigurationContext
import com.intellij.execution.actions.ConfigurationFromContext
import com.intellij.execution.actions.LazyRunConfigurationProducer
import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.openapi.util.Ref
import com.intellij.openapi.util.io.FileUtil
import com.intellij.psi.PsiElement
import com.intellij.util.PathUtil
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.nodes.getProjectModelNode
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.run.configurations.dotNetExe.DotNetExeConfiguration
import com.jetbrains.rider.run.configurations.dotNetExe.DotNetExeConfigurationType

class T4RunConfigurationProducer : LazyRunConfigurationProducer<DotNetExeConfiguration>() {
  private val configurationType: DotNetExeConfigurationType = DotNetExeConfigurationType()

  override fun getConfigurationFactory() = T4ConfigurationFactory(configurationType)

  override fun isConfigurationFromContext(
    configuration: DotNetExeConfiguration,
    context: ConfigurationContext
  ): Boolean {
    return true
    val node = context.dataContext.getProjectModelNode()
    val t4File = node?.getVirtualFile() ?: return false
    val path = t4File.path
    return FileUtil.toSystemIndependentName(path) == configuration.parameters.exePath
  }

  override fun setupConfigurationFromContext(
    configuration: DotNetExeConfiguration,
    context: ConfigurationContext,
    sourceElement: Ref<PsiElement>
  ): Boolean {
    val t4File = sourceElement.get().containingFile as? T4PsiFile ?: return false
    val model = configuration.project.solution.t4ProtocolModel
    val path = t4File.virtualFile.path
    val protocolConfiguration = model.executableConfigurations[path] ?: return false

    configuration.name = t4File.name
    configuration.parameters.exePath = protocolConfiguration.executablePath
    configuration.parameters.programParameters = protocolConfiguration.outputName
    configuration.parameters.isPassParentEnvs = false
    configuration.parameters.runtimeArguments = ""
    configuration.parameters.useMonoRuntime = false
    configuration.parameters.envs = emptyMap()
    configuration.parameters.workingDirectory = PathUtil.getParentPath(path)
    configuration.beforeRunTasks = emptyList()

    return true
  }

  override fun cloneTemplateConfiguration(context: ConfigurationContext): RunnerAndConfigurationSettings {
    return super.cloneTemplateConfiguration(context)
  }

  override fun createConfigurationFromContext(context: ConfigurationContext): ConfigurationFromContext? {
    return super.createConfigurationFromContext(context)
  }

  override fun createLightConfiguration(context: ConfigurationContext): RunConfiguration? {
    return super.createLightConfiguration(context)
  }

  override fun findExistingConfiguration(context: ConfigurationContext): RunnerAndConfigurationSettings? {
    return super.findExistingConfiguration(context)
  }

  override fun findOrCreateConfigurationFromContext(context: ConfigurationContext): ConfigurationFromContext? {
    return super.findOrCreateConfigurationFromContext(context)
  }

  override fun getConfigurationSettingsList(runManager: RunManager): MutableList<RunnerAndConfigurationSettings> {
    return super.getConfigurationSettingsList(runManager)
  }

  override fun isPreferredConfiguration(self: ConfigurationFromContext?, other: ConfigurationFromContext?): Boolean {
    return super.isPreferredConfiguration(self, other)
  }

  override fun onFirstRun(
    configuration: ConfigurationFromContext,
    context: ConfigurationContext,
    startRunnable: Runnable
  ) {
    super.onFirstRun(configuration, context, startRunnable)
  }

  override fun shouldReplace(self: ConfigurationFromContext, other: ConfigurationFromContext): Boolean {
    return super.shouldReplace(self, other)
  }
}
