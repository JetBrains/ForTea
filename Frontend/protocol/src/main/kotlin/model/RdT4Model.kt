package model

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.SolutionModel

@Suppress("unused")
object T4ProtocolModel : Ext(SolutionModel.Solution) {
  val T4BuildMessageKind = enum {
    +"Message"
    +"Success"
    +"Warning"
    +"Error"
  }

  val T4Location = structdef {
    field("line", int)
    field("column", int)
  }

  val T4BuildMessage = structdef {
    field("buildMessageKind", T4BuildMessageKind)
    field("id", string)
    field("location", T4Location)
    field("content", string)
    field("projectId", int)
    field("file", string.nullable)
  }

  val T4BuildResultKind = enum {
    +"HasErrors"
    +"HasWarnings"
    +"Successful"
  }

  val T4BuildResult = structdef {
    field("buildResultKind", T4BuildResultKind)
    field("messages", immutableList(T4BuildMessage))
  }

  val T4ConfigurationModel = structdef {
    field("executablePath", string)
    field("outputPath", string)
  }

  val T4FileLocation = structdef {
    field("id", int)
  }

  val T4PreprocessingResult = structdef {
    field("location", T4FileLocation)
    field("succeeded", bool)
    field("message", T4BuildMessage.nullable)
  }

  val T4ExecutionRequest = structdef {
    field("location", T4FileLocation)
    field("isVisible", bool)
  }

  init {
    // When this signal is received, tool window is displayed
    signal("preprocessingFinished", T4PreprocessingResult).async

    // Backend calls these to create and run new configurations
    val requestExecution = call("requestExecution", T4ExecutionRequest, void).async
    requestExecution.flow = FlowKind.Sink
    val requestDebug = call("requestDebug", T4ExecutionRequest, void).async
    requestDebug.flow = FlowKind.Sink

    call("getConfiguration", T4FileLocation, T4ConfigurationModel).async

    // Frontend calls this before executing file
    call("requestCompilation", T4FileLocation, T4BuildResult).async
    signal("executionSucceeded", T4FileLocation).async
    signal("executionFailed", T4FileLocation).async
    signal("executionAborted", T4FileLocation).async

    call("getProjectDependencies", T4FileLocation, immutableList(int)).async
  }
}
