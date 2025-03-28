package com.jetbrains.fortea.configuration.run.task

import com.intellij.execution.BeforeRunTaskProvider
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.icons.AllIcons
import com.intellij.openapi.actionSystem.DataContext
import com.intellij.openapi.util.Key
import com.intellij.openapi.vfs.VirtualFile
import com.intellij.util.application
import com.intellij.util.concurrency.Semaphore
import com.intellij.platform.backend.workspace.WorkspaceModel
import com.intellij.platform.backend.workspace.virtualFile
import com.jetbrains.fortea.configuration.run.T4RunConfiguration
import com.jetbrains.fortea.model.t4ProtocolModel
import com.jetbrains.fortea.utils.RiderT4Bundle
import com.jetbrains.fortea.utils.handleEndOfExecution
import com.jetbrains.rider.build.BuildHost
import com.jetbrains.rider.build.BuildParameters
import com.jetbrains.rider.model.BuildResultKind
import com.jetbrains.rider.model.BuildTarget
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.projectView.workspace.getProjectModelEntity
import javax.swing.Icon

class T4BuildProjectsBeforeRunTaskProvider : BeforeRunTaskProvider<T4BuildProjectsBeforeRunTask>() {
  override fun getId() = providerId
  override fun getName() = RiderT4Bundle.message("task.build.project.title")
  override fun getDescription(task: T4BuildProjectsBeforeRunTask) = RiderT4Bundle.message("task.build.project.description")
  override fun getIcon(): Icon = AllIcons.Actions.Compile

  override fun createTask(runConfiguration: RunConfiguration): T4BuildProjectsBeforeRunTask? {
    if (runConfiguration !is T4RunConfiguration) return null
    val task = T4BuildProjectsBeforeRunTask()
    task.isEnabled = true
    return task
  }

  override fun executeTask(
    context: DataContext,
    configuration: RunConfiguration,
    env: ExecutionEnvironment,
    task: T4BuildProjectsBeforeRunTask
  ): Boolean {
    val project = configuration.project
    val buildHost = BuildHost.getInstance(project)
    if (configuration !is T4RunConfiguration) return false
    val model = project
      .solution
      .t4ProtocolModel
    val selectedProjectsForBuild = model
      .getProjectDependencies
      .sync(configuration.parameters.request.location)
      .mapNotNull { WorkspaceModel.getInstance(project).getProjectModelEntity(it) }
      .mapNotNull { it.url?.virtualFile }
      .map(VirtualFile::getPath)
    if (selectedProjectsForBuild.isEmpty()) return true
    val finished = Semaphore()
    finished.down()
    var result = false
    // when false returned build was not started because another is in progress, we should not run task
    application.invokeLater {
      result = buildHost.requestBuild(BuildParameters(BuildTarget(), selectedProjectsForBuild, silentMode = true)) {
        result = it == BuildResultKind.Successful || it == BuildResultKind.HasWarnings
        finished.up()
      }
    }
    finished.waitFor()
    if (!result) model.executionAborted.handleEndOfExecution(configuration.parameters.request.location)
    return result
  }

  companion object {
    val providerId = Key.create<T4BuildProjectsBeforeRunTask>("Build before T4")
  }
}
