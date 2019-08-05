package com.jetbrains.fortea.configuration.actions

import com.intellij.openapi.project.Project
import com.intellij.util.PathUtil
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
import com.jetbrains.fortea.configuration.run.T4RunConfigurationFactory
import com.jetbrains.fortea.configuration.run.T4RunConfigurationType
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.solution
import javax.swing.Icon

abstract class T4RunTemplateActionBase(
  name: String,
  icon: Icon
) : T4FileBasedActionBase<T4RunConfiguration, T4RunConfigurationType>(name, icon) {
  final override fun createConfiguration(project: Project, configurationType: T4RunConfigurationType) =
    T4RunConfiguration("", project, configurationType.factory, T4RunConfigurationFactory.createParameters())

  final override fun setupFromFile(configuration: T4RunConfiguration, file: T4PsiFile) {
    val model = configuration.project.solution.t4ProtocolModel
    val t4Path = file.virtualFile.path
    val protocolConfiguration = model.configurations[t4Path]!!

    configuration.name = file.name
    configuration.parameters.exePath = protocolConfiguration.executablePath
    configuration.parameters.programParameters = protocolConfiguration.outputPath
    configuration.parameters.isPassParentEnvs = false
    configuration.parameters.runtimeArguments = ""
    configuration.parameters.useMonoRuntime = false
    configuration.parameters.envs = emptyMap()
    configuration.parameters.workingDirectory = PathUtil.getParentPath(t4Path)
    configuration.parameters.initialFilePath = t4Path
  }

  final override val configurationTypeClass = T4RunConfigurationType::class
}
