package com.jetbrains.fortea.configuration.actions

import com.intellij.execution.Executor
import com.intellij.execution.executors.DefaultDebugExecutor
import com.jetbrains.rider.icons.ReSharperCommonIcons

class T4DebugTemplateAction : T4RunTemplateActionBase("Debug T4 Template", ReSharperCommonIcons.Debug) {
  override val executor: Executor = DefaultDebugExecutor.getDebugExecutorInstance()
}
