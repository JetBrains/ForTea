package com.jetbrains.fortea.configuration

import com.intellij.openapi.project.Project
import com.jetbrains.rd.platform.util.getComponent
import com.jetbrains.rd.platform.util.lifetime
import com.jetbrains.rd.util.reactive.advise
import com.jetbrains.rider.model.T4PreprocessingResult
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution

class T4PreprocessNotificationManager(
  private val project: Project,
  private val host: ProjectModelViewHost
) {
  init {
    val model = project.solution.t4ProtocolModel
    model.preprocessingStarted.advise(project.lifetime, ::onPreprocessStarted)
    model.preprocessingFinished.advise(project.lifetime, ::onPreprocessFinished)
  }

  private fun onPreprocessFinished(result: T4PreprocessingResult) {
    val view = project.getComponent<T4BuildSessionView>()
    val path = host.getItemById(result.location.id)?.getVirtualFile()?.path ?: return
    view.showT4PreprocessingResult(result, path)
  }

  private fun onPreprocessStarted() {
    val view = project.getComponent<T4BuildSessionView>()
    view.openWindow("T4 Preprocessing Started...")
  }
}
