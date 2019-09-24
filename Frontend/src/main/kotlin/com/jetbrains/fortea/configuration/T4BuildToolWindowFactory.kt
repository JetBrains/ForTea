package com.jetbrains.fortea.configuration

import com.intellij.icons.AllIcons
import com.intellij.ide.actions.CloseTabToolbarAction
import com.intellij.openapi.actionSystem.ActionManager
import com.intellij.openapi.actionSystem.ActionPlaces
import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.actionSystem.DefaultActionGroup
import com.intellij.openapi.application.Application
import com.intellij.openapi.project.Project
import com.intellij.openapi.wm.impl.status.StatusBarUtil
import com.intellij.ui.content.Content
import com.intellij.ui.content.ContentManager
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rdclient.util.idea.LifetimedProjectService
import com.jetbrains.rider.build.BuildToolWindowContext
import com.jetbrains.rider.build.BuildToolWindowFactory
import com.jetbrains.rider.build.ui.BuildResultPanel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import java.awt.BorderLayout
import javax.swing.JPanel

class T4BuildToolWindowFactory(
  project: Project,
  private val buildToolWindowFactory: BuildToolWindowFactory,
  private val projectModelViewHost: ProjectModelViewHost,
  val application: Application
) : LifetimedProjectService(project) {
  private val lock = Any()
  private var context: BuildToolWindowContext? = null

  fun getOrCreateContext(lifetime: Lifetime, windowHeader: String): BuildToolWindowContext {
    synchronized(lock) {
      return context ?: create(lifetime, windowHeader)
    }
  }

  private fun create(lifetime: Lifetime, windowHeader: String): BuildToolWindowContext {
    val toolWindow = buildToolWindowFactory.getOrRegisterToolWindow()
    val contentManager = toolWindow.contentManager
    toolWindow.icon = AllIcons.Toolwindows.ToolWindowBuild
    // Required for hiding window without content
    val panel = BuildResultPanel(project, projectModelViewHost, componentLifetime)
    val toolWindowContent = contentManager.factory.createContent(null, windowHeader, true).apply {
      StatusBarUtil.setStatusBarInfo(project, "")
      component = panel
      isCloseable = false
    }
    panel.toolbar = createToolbarPanel(panel, contentManager, toolWindowContent)

    contentManager.addContent(toolWindowContent)
    val ctx = BuildToolWindowContext(toolWindow, toolWindowContent, panel)
    lifetime.bracket({ context = ctx }, {
      synchronized(lock) {
        contentManager.removeContent(toolWindowContent, true)
        context = null
      }
    })
    return ctx
  }

  private fun createToolbarPanel(
    buildResultPanel: BuildResultPanel,
    contentManager: ContentManager,
    toolWindowContent: Content
  ): JPanel {
    val buildActionGroup = DefaultActionGroup().apply {
      add(object : CloseTabToolbarAction() {
        override fun update(e: AnActionEvent) {
          e.presentation.isEnabled = true
        }

        override fun actionPerformed(e: AnActionEvent) {
          contentManager.removeContent(toolWindowContent, true)
          context = null
        }
      })
      buildResultPanel.showEvents()
    }
    return JPanel(BorderLayout()).apply {
      add(
        ActionManager.getInstance().createActionToolbar(
          ActionPlaces.COMPILER_MESSAGES_TOOLBAR,
          buildActionGroup,
          false
        ).component, BorderLayout.WEST
      )
    }
  }
}
