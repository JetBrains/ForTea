package com.jetbrains.fortea.configuration

import com.intellij.openapi.project.Project
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.reactive.ViewableMap
import com.jetbrains.rdclient.util.idea.LifetimedProjectService
import com.jetbrains.rider.build.BuildToolWindowContext
import com.jetbrains.rider.build.Diagnostics.BuildDiagnostic
import com.jetbrains.rider.build.Diagnostics.DiagnosticKind
import com.jetbrains.rider.model.BuildMessageKind
import com.jetbrains.rider.model.MessageBuildEvent
import com.jetbrains.rider.model.T4BuildMessage
import com.jetbrains.rider.model.T4BuildMessageKind

class T4BuildSessionView(
  project: Project,
  private val windowFactory: T4BuildToolWindowFactory
) : LifetimedProjectService(project) {
  fun openWindow(lifetime: Lifetime) = windowFactory.application.invokeLater {
    val context = initializeContext(lifetime)
    context.clear()
    val buildEvent = MessageBuildEvent(null, BuildMessageKind.Message, "Build started")
    context.addOutputMessage(buildEvent, ViewableMap())
    context.invalidatePanelMode()
  }

  fun showT4BuildResult(lifetime: Lifetime, buildMessages: List<T4BuildMessage>, file: String) {
    val context = initializeContext(lifetime)
    val buildDiagnostics = buildMessages.map {
      toBuildDiagnostic(it, file)
    }
    for (it in buildDiagnostics) {
      context.addBuildEvent(it)
    }
    context.invalidatePanelMode()
  }

  private fun initializeContext(lifetime: Lifetime): BuildToolWindowContext {
    val context = windowFactory.getOrCreateContext(lifetime)
    if (!context.isActive) context.showToolWindowIfHidden(true)
    return context
  }

  private fun toBuildDiagnostic(
    message: T4BuildMessage,
    file: String
  ): BuildDiagnostic {
    val kind = toDiagnosticKind(message.buildMessageKind)
    val line = message.location.line + 1
    val column = message.location.column + 1
    return BuildDiagnostic(kind, message.content, message.id, message.projectId, message.file ?: file, line, column)
  }

  private fun toDiagnosticKind(kind: T4BuildMessageKind) = when (kind) {
    T4BuildMessageKind.Error -> DiagnosticKind.Error
    T4BuildMessageKind.Warning -> DiagnosticKind.Warning
    T4BuildMessageKind.Message -> DiagnosticKind.Warning // ?
    T4BuildMessageKind.Success -> DiagnosticKind.Warning // ?
  }
}
