package com.jetbrains.fortea.configuration.impl

import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.T4BuildSessionView
import com.jetbrains.fortea.configuration.T4BuildToolWindowFactory
import com.jetbrains.fortea.configuration.toBuildResultKind
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.lifetime.SequentialLifetimes
import com.jetbrains.rd.util.reactive.ViewableMap
import com.jetbrains.rdclient.util.idea.LifetimedProjectService
import com.jetbrains.rider.build.BuildToolWindowContext
import com.jetbrains.rider.build.Diagnostics.BuildDiagnostic
import com.jetbrains.rider.build.Diagnostics.DiagnosticKind
import com.jetbrains.rider.model.*
import com.jetbrains.rider.util.idea.lifetime
import javax.management.OperationsException

class T4BuildSessionViewImpl(
  project: Project,
  private val windowFactory: T4BuildToolWindowFactory
) : LifetimedProjectService(project), T4BuildSessionView {
  private val windowLifetimes = SequentialLifetimes(project.lifetime)
  private var currentWindowLifetime: Lifetime? = null

  override fun openWindow(message: String) = windowFactory.application.invokeLater {
    val lifetime = windowLifetimes.next()
    currentWindowLifetime = lifetime
    val context = initializeContext(lifetime, ExecutingT4BuildHeader)
    context.clear()
    context.showToolWindowIfHidden(true)
    val buildEvent = MessageBuildEvent(null, BuildMessageKind.Message, message)
    context.addOutputMessage(buildEvent, ViewableMap())
    context.invalidatePanelMode()
  }

  override fun showT4BuildResult(result: T4BuildResult, file: String) = windowFactory.application.invokeLater {
    val lifetime = currentWindowLifetime ?: throw OperationsException("openWindow should be called first")
    val context = initializeContext(lifetime, ExecutingT4BuildHeader)
    context.updateStatus(result.buildResultKind.toBuildResultKind, T4BuildHeader)
    showMessages(result, file, context)
    context.invalidatePanelMode()
  }

  override fun showT4PreprocessingResult(result: T4PreprocessingResult, file: String) = windowFactory.application.invokeLater {
    val lifetime = currentWindowLifetime ?: throw OperationsException("openWindow should be called first")
    val context = initializeContext(lifetime, PreprocessingT4Header)
    val succeeded =
      if (result.succeeded) BuildResultKind.Successful
      else BuildResultKind.HasErrors
    context.updateStatus(succeeded, T4PreprocessingHeader)
    result.message.map { toBuildDiagnostic(it, file) }.forEach(context::addBuildEvent)
    context.invalidatePanelMode()
  }

  private fun showMessages(
    result: T4BuildResult,
    file: String,
    context: BuildToolWindowContext
  ) = result.messages.map {
    toBuildDiagnostic(it, file)
  }.forEach(context::addBuildEvent)

  private fun initializeContext(lifetime: Lifetime, windowHeader: String): BuildToolWindowContext {
    val context = windowFactory.getOrCreateContext(lifetime, windowHeader)
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
    val filePath = if (!message.file.isNullOrEmpty()) message.file else file
    return BuildDiagnostic(kind, message.content, message.id, message.projectId, filePath, line, column)
  }

  private fun toDiagnosticKind(kind: T4BuildMessageKind) = when (kind) {
    T4BuildMessageKind.Error -> DiagnosticKind.Error
    T4BuildMessageKind.Warning -> DiagnosticKind.Warning
    T4BuildMessageKind.Message -> DiagnosticKind.Warning // ?
    T4BuildMessageKind.Success -> DiagnosticKind.Warning // ?
  }

  companion object {
    const val ExecutingT4BuildHeader = "Executing T4 Build..."
    const val PreprocessingT4Header = "Preprocessing T4..."
    const val T4BuildHeader = "T4 Build"
    const val T4PreprocessingHeader = "T4 Preprocessing"
  }
}
