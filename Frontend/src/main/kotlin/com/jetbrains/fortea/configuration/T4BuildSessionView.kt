package com.jetbrains.fortea.configuration

import com.intellij.notification.NotificationGroup
import com.intellij.openapi.project.Project
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.lifetime.isAlive
import com.jetbrains.rdclient.util.idea.LifetimedProjectService
import com.jetbrains.rider.build.Diagnostics.BuildDiagnostic
import com.jetbrains.rider.build.Diagnostics.DiagnosticKind
import com.jetbrains.rider.model.T4BuildMessage
import com.jetbrains.rider.model.T4BuildMessageKind
import com.jetbrains.rider.util.idea.getLogger

class T4BuildSessionView(
  project: Project,
  private val windowFactory: T4BuildToolWindowFactory
) : LifetimedProjectService(project) {
  companion object {
    val BUILD_NOTIFICATION_GROUP =
      NotificationGroup.toolWindowGroup("T4 Build Messages", T4BuildToolWindowFactory.TOOLWINDOW_ID)
    private val myLogger = getLogger<T4BuildSessionView>()
  }

  fun showT4BuildResult(lifetime: Lifetime, buildMessages: List<T4BuildMessage>, file: String) {
    val context = windowFactory.getOrCreateContext(lifetime)
    context.clear()
    if (!lifetime.isAlive) return
    val shouldReactivateBuildToolWindow = !context.isActive
    val buildDiagnostics = buildMessages.map {
      toBuildDiagnostic(it, file)
    }
    for (it in buildDiagnostics) {
      context.addBuildEvent(it)
    }
    if (shouldReactivateBuildToolWindow) context.showToolWindowIfHidden(true)
    context.invalidatePanelMode()
  }

  private fun toBuildDiagnostic(
    message: T4BuildMessage,
    file: String
  ): BuildDiagnostic {
    val kind = toDiagnosticKind(message.buildMessageKind)
    val line = message.location.line + 1
    val column = message.location.column + 1
    return BuildDiagnostic(kind, message.content, message.id, 42, file, line, column)
  }

  private fun toDiagnosticKind(kind: T4BuildMessageKind) = when (kind) {
    T4BuildMessageKind.Error -> DiagnosticKind.Error
    T4BuildMessageKind.Warning -> DiagnosticKind.Warning
    T4BuildMessageKind.Message -> DiagnosticKind.Warning // ?
    T4BuildMessageKind.Success -> DiagnosticKind.Warning // ?
  }
}
