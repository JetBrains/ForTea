package com.jetbrains.fortea.configuration.impl

import com.intellij.openapi.components.service
import com.intellij.openapi.project.Project
import com.intellij.util.application
import com.jetbrains.fortea.configuration.T4BuildSessionView
import com.jetbrains.fortea.configuration.T4BuildToolWindowFactory
import com.jetbrains.fortea.configuration.toBuildResultKind
import com.jetbrains.fortea.model.T4BuildMessage
import com.jetbrains.fortea.model.T4BuildMessageKind
import com.jetbrains.fortea.model.T4BuildResult
import com.jetbrains.fortea.model.T4PreprocessingResult
import com.jetbrains.fortea.utils.RiderT4Bundle
import com.jetbrains.rd.platform.util.idea.LifetimedService
import com.jetbrains.rd.util.reactive.ViewableMap
import com.jetbrains.rider.build.diagnostics.BuildDiagnostic
import com.jetbrains.rider.build.diagnostics.DiagnosticKind
import com.jetbrains.rider.build.ui.old.BuildToolWindowContext
import com.jetbrains.rider.model.BuildMessageKind
import com.jetbrains.rider.model.BuildResultKind
import com.jetbrains.rider.model.MessageBuildEvent

class T4BuildSessionViewImpl(private val project: Project) : LifetimedService(), T4BuildSessionView {
  override fun openWindow(message: String) = application.invokeLater {
    val context = initializeContext(ExecutingT4BuildHeader)
    context.clear()
    context.showToolWindowIfHidden(true)
    val buildEvent = MessageBuildEvent(null, BuildMessageKind.Message, message)
    context.addOutputMessage(buildEvent, ViewableMap())
    context.invalidatePanelMode()
  }

  override fun showT4BuildResult(result: T4BuildResult, file: String) = application.invokeLater {
    val context = initializeContext(ExecutingT4BuildHeader)
    context.updateStatus(result.buildResultKind.toBuildResultKind, T4BuildHeader)
    showMessages(result, file, context)
    context.invalidatePanelMode()
  }

  override fun showT4PreprocessingResult(result: T4PreprocessingResult, file: String) = application.invokeLater {
    val context = initializeContext(PreprocessingT4Header)
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

  private fun initializeContext(windowHeader: String): BuildToolWindowContext {
    val context = project.service<T4BuildToolWindowFactory>().getOrCreateContext(windowHeader)
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
    return BuildDiagnostic(kind, message.content, message.id, message.projectId, null, filePath, line, column)
  }

  private fun toDiagnosticKind(kind: T4BuildMessageKind) = when (kind) {
    T4BuildMessageKind.Error -> DiagnosticKind.Error
    T4BuildMessageKind.Warning -> DiagnosticKind.Warning
    T4BuildMessageKind.Message -> DiagnosticKind.Warning // ?
    T4BuildMessageKind.Success -> DiagnosticKind.Warning // ?
  }

  companion object {
    val ExecutingT4BuildHeader = RiderT4Bundle.message("status.executing.t4.build")
    val PreprocessingT4Header = RiderT4Bundle.message("status.preprocessing.t4")
    val T4BuildHeader = RiderT4Bundle.message("status.t4.build")
    val T4PreprocessingHeader = RiderT4Bundle.message("status.t4.preprocessing")
  }
}
