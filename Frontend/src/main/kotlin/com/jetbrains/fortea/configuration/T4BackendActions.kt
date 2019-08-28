package com.jetbrains.fortea.configuration

import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.actionSystem.CommonDataKeys
import com.jetbrains.fortea.language.T4Language
import com.jetbrains.rider.actions.base.RiderContextAwareAnAction
import com.jetbrains.rider.icons.ReSharperCommonIcons.Debug
import com.jetbrains.rider.icons.ReSharperLiveTemplatesCSharpIcons.ScopeCS
import com.jetbrains.rider.icons.ReSharperPsiBuildScriptsIcons.Run
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.model.t4ProtocolModel
import com.jetbrains.rider.projectView.ProjectModelViewHost
import com.jetbrains.rider.projectView.solution
import javax.swing.Icon

abstract class T4BackendAction(backendActionId: String, icon: Icon) :
  RiderContextAwareAnAction(backendActionId, icon = icon) {
  override fun update(e: AnActionEvent) {
    e.presentation.isEnabledAndVisible = e.dataContext.getData(CommonDataKeys.PSI_FILE)?.language == T4Language
  }
}

abstract class T4ExecutionBackendAction(backendActionId: String, icon: Icon) : T4BackendAction(backendActionId, icon) {
  override fun update(e: AnActionEvent) {
    super.update(e)
    if (!e.presentation.isVisible) return
    e.presentation.isEnabled = false
    val psiFile = e.dataContext.getData(CommonDataKeys.PSI_FILE) ?: return
    val project = e.project ?: return
    val host = ProjectModelViewHost.getInstance(project)
    val item = host.getItemsByVirtualFile(psiFile.virtualFile).singleOrNull() ?: return
    val model = project.solution.t4ProtocolModel
    val canExecute = model.canExecute.sync(T4FileLocation(item.id))
    if (canExecute) e.presentation.isEnabled = true
  }
}

class T4ExecuteTemplateBackendAction : T4ExecutionBackendAction("T4.ExecuteFromContext", Run)
class T4DebugTemplateBackendAction : T4ExecutionBackendAction("T4.DebugFromContext", Debug)
class T4PreprocessTemplateBackendAction : T4BackendAction("T4.PreprocessFromContext", ScopeCS)
