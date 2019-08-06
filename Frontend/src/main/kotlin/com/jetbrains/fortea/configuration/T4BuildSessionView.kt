package com.jetbrains.fortea.configuration

import com.intellij.notification.NotificationGroup
import com.intellij.openapi.project.Project
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.lifetime.isAlive
import com.jetbrains.rdclient.util.idea.LifetimedProjectService
import com.jetbrains.rider.build.Diagnostics.BuildDiagnostic
import com.jetbrains.rider.build.Diagnostics.DiagnosticKind
import com.jetbrains.rider.model.BuildMessage
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

  fun showT4BuildResult(lifetime: Lifetime, buildMessages: List<BuildMessage>, file: String) {
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
    message: BuildMessage,
    file: String
  ) = BuildDiagnostic(toDiagnosticKind(message.buildMessageKind), message.message, "CS0042", 42, file, 0, 0)

  private fun toDiagnosticKind(kind: T4BuildMessageKind) = when (kind) {
    T4BuildMessageKind.T4Error -> DiagnosticKind.Error
    T4BuildMessageKind.T4Warning -> DiagnosticKind.Warning
    T4BuildMessageKind.T4Message -> DiagnosticKind.Warning // ?
    T4BuildMessageKind.T4Success -> DiagnosticKind.Warning // ?
  }
}
