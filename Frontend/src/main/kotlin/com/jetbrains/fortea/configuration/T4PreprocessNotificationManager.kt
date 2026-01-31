package com.jetbrains.fortea.configuration

import com.intellij.openapi.project.Project
import com.intellij.platform.backend.workspace.WorkspaceModel
import com.intellij.workspaceModel.ide.toPath
import com.jetbrains.fortea.model.T4PreprocessingResult
import com.jetbrains.fortea.model.t4ProtocolModel
import com.jetbrains.fortea.utils.RiderT4Bundle
import com.jetbrains.rd.platform.util.getComponent
import com.jetbrains.rd.platform.util.lifetime
import com.jetbrains.rd.util.reactive.advise
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.projectView.workspace.getProjectModelEntity

class T4PreprocessNotificationManager(private val project: Project) {
  init {
    val model = project.solution.t4ProtocolModel
    model.preprocessingStarted.advise(project.lifetime, ::onPreprocessStarted)
    model.preprocessingFinished.advise(project.lifetime, ::onPreprocessFinished)
  }

  private fun onPreprocessFinished(result: T4PreprocessingResult) {
    val view = project.getComponent<T4BuildSessionView>()
    val path = WorkspaceModel.getInstance(project).getProjectModelEntity(result.location.id)?.url?.toPath()?.toString() ?: return
    view.showT4PreprocessingResult(result, path)
  }

  private fun onPreprocessStarted() {
    val view = project.getComponent<T4BuildSessionView>()
    view.openWindow(RiderT4Bundle.message("status.t4.preprocessing.started"))
  }
}
