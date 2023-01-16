package com.jetbrains.fortea.configuration

import com.intellij.icons.AllIcons
import com.intellij.ide.actions.CloseTabToolbarAction
import com.intellij.openapi.actionSystem.ActionManager
import com.intellij.openapi.actionSystem.ActionPlaces
import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.actionSystem.DefaultActionGroup
import com.intellij.openapi.project.Project
import com.intellij.openapi.wm.impl.status.StatusBarUtil
import com.intellij.ui.content.Content
import com.intellij.ui.content.ContentManager
import com.jetbrains.rd.platform.util.idea.LifetimedService
import com.jetbrains.rider.build.BuildToolWindowContext
import com.jetbrains.rider.build.BuildToolWindowFactory
import com.jetbrains.rider.build.redesign.BuildToolwindowWidget
import com.jetbrains.rider.util.idea.getService
import java.awt.BorderLayout
import javax.swing.JPanel

class T4BuildToolWindowFactory(private val project: Project) : LifetimedService() {
  private val lock = Any()
  private var context: BuildToolWindowContext? = null

  fun getOrCreateContext(windowHeader: String): BuildToolWindowContext {
    synchronized(lock) {
      return context ?: create(windowHeader)
    }
  }

  private fun create(windowHeader: String): BuildToolWindowContext {
    val toolWindow = BuildToolWindowFactory.getInstance(project).getOrRegisterToolWindow(project)
    val contentManager = toolWindow.contentManager
    toolWindow.setIcon(AllIcons.Toolwindows.ToolWindowBuild)
    // Required for hiding window without content
    val panel = BuildToolwindowWidget(project, serviceLifetime) //BuildResultPanel(project, serviceLifetime)
    val toolWindowContent = contentManager.factory.createContent(null, windowHeader, true).apply {
      StatusBarUtil.setStatusBarInfo(project, "")
      component = panel
      isCloseable = false
    }
    //panel.toolbar = createToolbarPanel(panel, contentManager, toolWindowContent)

    contentManager.addContent(toolWindowContent)
    val ctx = BuildToolWindowContext(toolWindow, toolWindowContent, panel)
    context = ctx
    return ctx
  }

  private fun createToolbarPanel(
    buildResultPanel: BuildToolwindowWidget,
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
      //TODO[sbe] ? buildResultPanel.showEvents()
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

  companion object {
    fun getInstance(project: Project): T4BuildToolWindowFactory = project.getService()
  }
}
