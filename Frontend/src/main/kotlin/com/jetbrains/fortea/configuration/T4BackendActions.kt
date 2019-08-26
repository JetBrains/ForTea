package com.jetbrains.fortea.configuration

import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.actionSystem.CommonDataKeys
import com.jetbrains.fortea.language.T4Language
import com.jetbrains.rider.actions.base.RiderContextAwareAnAction
import com.jetbrains.rider.icons.ReSharperLiveTemplatesCSharpIcons.ScopeCS
import com.jetbrains.rider.icons.ReSharperPsiBuildScriptsIcons.Run
import javax.swing.Icon

abstract class T4BackendAction(backendActionId: String, icon: Icon) :
  RiderContextAwareAnAction(backendActionId, icon = icon) {
  override fun update(e: AnActionEvent) {
    val dataContext = e.dataContext
    val presentation = e.presentation
    if (dataContext.getData(CommonDataKeys.PSI_FILE)?.language != T4Language) {
      presentation.isEnabledAndVisible = false
      return
    }
    super.update(e)
  }
}

class T4ExecuteTemplateBackendAction : T4BackendAction("T4.ExecuteFromContext", Run)
class T4DebugTemplateBackendAction : RiderContextAwareAnAction("T4.DebugFromContext")
class T4PreprocessTemplateBackendAction : T4BackendAction("T4.PreprocessFromContext", ScopeCS)
