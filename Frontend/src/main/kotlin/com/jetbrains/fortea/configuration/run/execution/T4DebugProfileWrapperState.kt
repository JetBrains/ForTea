package com.jetbrains.fortea.configuration.run.execution

import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.fortea.model.T4ProtocolModel
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rider.debugger.DebuggerHelperHost
import com.jetbrains.rider.run.IDotNetDebugProfileState
import com.jetbrains.rider.run.WorkerRunInfo

class T4DebugProfileWrapperState(
  private val wrappee: IDotNetDebugProfileState,
  private val model: T4ProtocolModel,
  private val parameters: T4RunConfigurationParameters
) : IDotNetDebugProfileState by wrappee {
  override suspend fun createWorkerRunInfo(lifetime: Lifetime, helper: DebuggerHelperHost, port: Int): WorkerRunInfo {
    val workerRunInfo = wrappee.createWorkerRunInfo(lifetime, helper, port)
    workerRunInfo.addProcessListener(T4PostProcessorProcessListener(model, parameters))
    return workerRunInfo
  }
}
