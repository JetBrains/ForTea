package com.jetbrains.fortea.utils

import com.jetbrains.fortea.configuration.execution.impl.T4SynchronousRunConfigurationExecutor
import com.jetbrains.rd.framework.IRdCall
import com.jetbrains.rider.util.idea.application

fun <TReq> IRdCall<TReq, Unit>.handleEndOfExecution(arg: TReq) {
  if (application.isUnitTestMode) sync(arg)
  else start(arg)
  T4SynchronousRunConfigurationExecutor.isExecutionRunning = false
}
