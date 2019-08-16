package com.jetbrains.fortea.configuration.preprocess.execution

import com.intellij.execution.DefaultExecutionResult
import com.intellij.execution.ExecutionResult
import com.intellij.execution.Executor
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.impl.ConsoleViewImpl
import com.intellij.execution.process.NopProcessHandler
import com.intellij.execution.runners.ProgramRunner
import com.intellij.execution.ui.ConsoleViewContentType
import com.intellij.openapi.project.Project
import com.intellij.util.PathUtil
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfigurationParameters
import com.jetbrains.rider.model.T4Location
import com.jetbrains.rider.model.T4PreprocessingResult
import com.jetbrains.rider.model.T4ProtocolModel
import com.jetbrains.rider.util.idea.lifetime

class T4PreprocessRunProfileState(
  private val parameters: T4PreprocessConfigurationParameters,
  private val model: T4ProtocolModel,
  private val project: Project
) : RunProfileState {
  override fun execute(executor: Executor?, runner: ProgramRunner<*>): ExecutionResult? {
    val console = ConsoleViewImpl(project, false)
    console.print("Preprocessing ${parameters.initialFileLocation.location}\n", ConsoleViewContentType.SYSTEM_OUTPUT)
    val handler = NopProcessHandler() // Pretend to be working
    model.requestPreprocessing.start(parameters.initialFileLocation).result.advise(project.lifetime) {
      val result = it.unwrap()
      if (!result.succeeded) console.printT4ErrorMessage(result)
      else console.print("Done\n", ConsoleViewContentType.NORMAL_OUTPUT)
      handler.destroyProcess()
    }
    return DefaultExecutionResult(console, handler)
  }

  private fun ConsoleViewImpl.printT4ErrorMessage(result: T4PreprocessingResult) {
    print("Failed:\n", ConsoleViewContentType.ERROR_OUTPUT)
    print(result.toWriteableMessage, ConsoleViewContentType.ERROR_OUTPUT)
    print("\n", ConsoleViewContentType.ERROR_OUTPUT)
  }

  private val T4PreprocessingResult.toWriteableMessage: String
    get() {

      val message = message ?: return "unknown error"
      val fileName = PathUtil.getFileName(parameters.initialFileLocation.location)
      return "$fileName${message.location.writeableMessage}: ${message.content}"
    }

  private val T4Location.writeableMessage
    get() = "($line, $column)"
}
