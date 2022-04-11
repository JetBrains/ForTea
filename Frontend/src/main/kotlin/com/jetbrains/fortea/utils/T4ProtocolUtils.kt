package com.jetbrains.fortea.utils

import com.intellij.util.application
import com.jetbrains.fortea.configuration.execution.impl.T4SynchronousRunConfigurationExecutor
import com.jetbrains.rd.framework.IRdCall
import com.jetbrains.rd.platform.util.lifetime

fun <TReq> IRdCall<TReq, Unit>.handleEndOfExecution(arg: TReq) {
  if (application.isUnitTestMode) sync(arg)
  else start(application.lifetime, arg)
  T4SynchronousRunConfigurationExecutor.isExecutionRunning = false
}
