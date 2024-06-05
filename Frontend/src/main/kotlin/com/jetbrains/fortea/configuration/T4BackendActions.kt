package com.jetbrains.fortea.configuration

import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.actionSystem.CommonDataKeys
import com.jetbrains.fortea.language.T4Language
import com.jetbrains.rider.actions.base.RiderContextAwareAnAction
import icons.ReSharperIcons.Common.Debug
import icons.ReSharperIcons.UnitTesting.RunTest
import javax.swing.Icon

abstract class T4BackendAction(backendActionId: String, icon: Icon? = null) :
  RiderContextAwareAnAction(backendActionId, icon = icon) {
  override fun update(e: AnActionEvent) {
    val psiFile = e.dataContext.getData(CommonDataKeys.PSI_FILE)
    if (psiFile?.language != T4Language) {
      e.presentation.isEnabledAndVisible = false
      return
    }
    super.update(e)
  }
}

class T4ExecuteTemplateBackendAction : T4BackendAction("T4ExecuteTemplate", RunTest)
class T4DebugTemplateBackendAction : T4BackendAction("T4DebugTemplate", Debug)
class T4PreprocessTemplateBackendAction : T4BackendAction("T4PreprocessTemplate")
