package com.jetbrains.fortea.configuration

import com.intellij.execution.ProgramRunnerUtil
import com.intellij.execution.RunManager
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.openapi.actionSystem.AnAction
import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.actionSystem.CommonDataKeys
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
import com.jetbrains.fortea.configuration.run.T4RunConfigurationFactory
import com.jetbrains.fortea.configuration.run.T4RunConfigurationProducer
import com.jetbrains.fortea.configuration.run.T4RunConfigurationType
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.icons.ReSharperCommonIcons

class T4DebugTemplateAction : AnAction("Debug T4 Template", null, ReSharperCommonIcons.Debug) {
  override fun update(event: AnActionEvent) {
    val t4File = CommonDataKeys.PSI_FILE.getData(event.dataContext) as? T4PsiFile
    val canSetup = if (t4File == null) false else T4RunConfigurationProducer.canSetup(t4File)
    event.presentation.isEnabledAndVisible = canSetup
  }

  override fun actionPerformed(e: AnActionEvent) {
    val project = e.project ?: return
    val t4File = CommonDataKeys.PSI_FILE.getData(e.dataContext) as? T4PsiFile ?: return
    val configurationType = ConfigurationTypeUtil.findConfigurationType(T4RunConfigurationType::class.java)
    val configuration =
      T4RunConfiguration("", project, configurationType.factory, T4RunConfigurationFactory.createParameters())
    assert(T4RunConfigurationProducer.setupFromFile(configuration, t4File))
    val configurationSettings =
      RunManager.getInstance(project).createConfiguration(configuration, configurationType.factory)
    val executor = DefaultDebugExecutor.getDebugExecutorInstance()
    ProgramRunnerUtil.executeConfiguration(configurationSettings, executor)
  }
}
