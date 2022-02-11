package com.jetbrains.fortea.configuration.execution.impl

import com.intellij.execution.*
import com.intellij.execution.executors.DefaultRunExecutor
import com.intellij.execution.process.NopProcessHandler
import com.intellij.execution.process.ProcessAdapter
import com.intellij.execution.process.ProcessEvent
import com.intellij.execution.runners.ExecutionEnvironmentBuilder
import com.intellij.openapi.diagnostic.logger
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.Key
import com.intellij.util.concurrency.Semaphore
import com.jetbrains.rdclient.util.idea.pumpMessages
import org.jetbrains.annotations.TestOnly

class T4SynchronousRunConfigurationExecutor(
  project: Project,
  private val waitFunc: () -> Boolean
) : T4RunConfigurationExecutorBase(project) {
  private val executor: Executor = DefaultRunExecutor.getRunExecutorInstance()

  override fun executeConfiguration(configuration: RunnerAndConfigurationSettings) {
    isExecutionRunning = true
    val builder: ExecutionEnvironmentBuilder
    try {
      builder = ExecutionEnvironmentBuilder.create(executor, configuration)
    } catch (e: ExecutionException) {
      logger.error(e)
      return
    }

    val finished = Semaphore()
    finished.down()

    val environment = builder.contentToReuse(null).dataContext(null).activeTarget().build()
    ProgramRunnerUtil.executeConfigurationAsync(
      environment,
      true,
      true
    ) {
      val processHandler = it.processHandler
      if (processHandler == null)
        logger.error("processHandler is null")

      processHandler?.addProcessListener(object : ProcessAdapter() {
        override fun processTerminated(event: ProcessEvent) {
          logger.info("T4 process terminated with exit code ${event.exitCode}.")
        }

        override fun onTextAvailable(event: ProcessEvent, outputType: Key<*>) {
          logger.info("T4 process $outputType: ${event.text}")
        }
      })

      val listener = project.messageBus.syncPublisher(ExecutionManager.EXECUTION_TOPIC)
      listener.processStarted(executor.id, environment, NopProcessHandler())
      finished.up()
    }
    waitPumping(finished)
  }

  private fun waitPumping(finished: Semaphore) {
    pumpMessages(waitFunc = waitFunc)
    finished.waitFor(10)
  }

  companion object {
    @get:TestOnly
    var isExecutionRunning = false

    private val logger = logger<T4SynchronousRunConfigurationExecutor>()
  }
}
