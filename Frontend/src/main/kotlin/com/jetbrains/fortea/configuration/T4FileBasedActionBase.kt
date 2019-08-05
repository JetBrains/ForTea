package com.jetbrains.fortea.configuration

import com.intellij.execution.Executor
import com.intellij.execution.ProgramRunnerUtil
import com.intellij.execution.RunManager
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.openapi.actionSystem.AnAction
import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.actionSystem.CommonDataKeys
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.psi.T4PsiFile
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.solution
import javax.swing.Icon
import kotlin.reflect.KClass

abstract class T4FileBasedActionBase<TConfiguration, TConfigurationType>(
  name: String,
  icon: Icon
) : AnAction(name, null, icon)
  where TConfiguration : RunConfiguration,
        TConfigurationType : T4ConfigurationType,
        TConfigurationType : Any {
  final override fun update(e: AnActionEvent) {
    val t4File = CommonDataKeys.PSI_FILE.getData(e.dataContext) as? T4PsiFile
    val canSetup = t4File != null && canSetup(t4File)
    e.presentation.isEnabledAndVisible = canSetup
  }

  private fun canSetup(file: T4PsiFile) =
    file.project.solution.t4ProtocolModel.configurations.containsKey(file.virtualFile.path)


  final override fun actionPerformed(e: AnActionEvent) {
    val project = e.project ?: return
    val file = CommonDataKeys.PSI_FILE.getData(e.dataContext) as? T4PsiFile ?: return
    val configurationType = ConfigurationTypeUtil.findConfigurationType(configurationTypeClass.java)
    val configuration = createConfiguration(project, configurationType)
    setupFromFile(configuration, file)
    val runManager = RunManager.getInstance(project)
    val configurationSettings = runManager.createConfiguration(configuration, configurationType.factory)
    ProgramRunnerUtil.executeConfiguration(configurationSettings, this.executor)
  }

  protected abstract fun createConfiguration(project: Project, configurationType: TConfigurationType): TConfiguration
  protected abstract fun setupFromFile(configuration: TConfiguration, file: T4PsiFile)
  protected abstract val configurationTypeClass: KClass<TConfigurationType>
  protected abstract val executor: Executor
}
