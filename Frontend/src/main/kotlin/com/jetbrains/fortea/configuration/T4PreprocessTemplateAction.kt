package com.jetbrains.fortea.configuration

import com.intellij.execution.ProgramRunnerUtil
import com.intellij.execution.RunManager
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.openapi.actionSystem.AnAction
import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.actionSystem.CommonDataKeys
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfiguration
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationParameters
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationProducer
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationType
import com.jetbrains.fortea.configuration.run.T4RunConfigurationProducer
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.icons.ReSharperAssemblyExplorerIcons

class T4PreprocessTemplateAction : AnAction(
  "Preprocess T4 Template",
  null,
  ReSharperAssemblyExplorerIcons.AddProcess
) {
  override fun update(event: AnActionEvent) {
    val t4File = CommonDataKeys.PSI_FILE.getData(event.dataContext) as? T4PsiFile
    val canSetup = if (t4File == null) false else T4RunConfigurationProducer.canSetup(t4File)
    event.presentation.isEnabledAndVisible = canSetup
  }

  override fun actionPerformed(e: AnActionEvent) {
    val project = e.project ?: return
    val t4File = CommonDataKeys.PSI_FILE.getData(e.dataContext) as? T4PsiFile ?: return
    val configurationType = ConfigurationTypeUtil.findConfigurationType(T4PreprocessConfigurationType::class.java)
    val configuration =
      T4PreprocessConfiguration("", project, configurationType.factory, T4PreprocessConfigurationParameters(""))
    assert(T4PreprocessConfigurationProducer.setupFromFile(configuration, t4File))
    val configurationSettings =
      RunManager.getInstance(project).createConfiguration(configuration, configurationType.factory)
    val executor = DefaultRunExecutor.getRunExecutorInstance()
    ProgramRunnerUtil.executeConfiguration(configurationSettings, executor)
  }
}
