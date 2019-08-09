package com.jetbrains.fortea.configuration.actions

import com.intellij.execution.Executor
import com.intellij.execution.executors.DefaultRunExecutor
import com.jetbrains.rider.icons.ReSharperPsiBuildScriptsIcons

class T4ExecuteTemplateAction : T4RunTemplateActionBase("Execute template", ReSharperPsiBuildScriptsIcons.Run) {
  override val executor: Executor = DefaultRunExecutor.getRunExecutorInstance()
}
