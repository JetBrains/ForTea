package com.jetbrains.fortea.configuration.run.execution

import com.intellij.execution.CantRunException
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.run.T4RunConfigurationParameters
import com.jetbrains.rd.platform.util.getComponent
import com.jetbrains.fortea.model.t4ProtocolModel
import com.jetbrains.rd.platform.util.lifetime
import com.jetbrains.rd.platform.util.startOnUiAsync
import com.jetbrains.rd.platform.util.toPromise
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rider.projectView.solution
import com.jetbrains.rider.run.configurations.IExecutorFactory
import com.jetbrains.rider.runtime.DotNetRuntime
import com.jetbrains.rider.runtime.RiderDotNetActiveRuntimeHost
import com.jetbrains.rider.debugger.DebuggerHelperHost
import org.jetbrains.concurrency.Promise

class T4ExecutorFactory(project: Project, private val parameters: T4RunConfigurationParameters) : IExecutorFactory {
  private val riderDotNetActiveRuntimeHost = project.getComponent<RiderDotNetActiveRuntimeHost>()
  private val debuggerHelperHost = DebuggerHelperHost.getInstance(project)
  @Deprecated("Synchronous call to DotNetExeExecutorFactory::create is not supported")
  override fun create(executorId: String, environment: ExecutionEnvironment): RunProfileState {
    throw UnsupportedOperationException("Synchronous call to DotNetExeExecutorFactory::create is not supported")
  }

  fun createAsync(executorId: String, environment: ExecutionEnvironment): Promise<RunProfileState> {
    return environment.project.lifetime.startOnUiAsync {
      val dotNetExecutable = parameters.toDotNetExecutable()
      val runtimeToExecute = DotNetRuntime.detectRuntimeForExeOrThrow(
        Lifetime.Eternal, // Don't care
        riderDotNetActiveRuntimeHost,
        debuggerHelperHost,
        dotNetExecutable.exePath,
        dotNetExecutable.runtimeType
      )
      val model = environment.project.solution.t4ProtocolModel
      return@startOnUiAsync when (executorId) {
          DefaultRunExecutor.EXECUTOR_ID -> {
            val wrappee = runtimeToExecute.createRunState(dotNetExecutable, environment)
            T4RunProfileWrapperState(wrappee, model, parameters)
          }
          DefaultDebugExecutor.EXECUTOR_ID -> {
            val wrappee = runtimeToExecute.createDebugState(dotNetExecutable, environment)
            T4DebugProfileWrapperState(wrappee, model, parameters)
          }
          else -> throw CantRunException("Unsupported executor $executorId")
      }
    }.toPromise()
  }
}
