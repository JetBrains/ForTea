package model

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.SolutionModel
import com.jetbrains.rd.generator.nova.csharp.CSharp50Generator
import com.jetbrains.rd.generator.nova.kotlin.Kotlin11Generator

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
    field("envDTEPort", int)
  }

  val T4FileLocation = structdef {
    field("id", int)
  }

  val T4PreprocessingResult = structdef {
    field("location", T4FileLocation)
    field("succeeded", bool)
    field("message", immutableList(T4BuildMessage))
  }

  val T4ExecutionRequest = structdef {
    field("location", T4FileLocation)
    field("isVisible", bool)
  }

  init {
    setting(Kotlin11Generator.Namespace, "com.jetbrains.fortea.model")
    setting(CSharp50Generator.Namespace, "JetBrains.ForTea.RiderPlugin.Model")

    // Backend calls these to create and run new configurations
    val requestExecution = call("requestExecution", T4ExecutionRequest, void).async
    requestExecution.flow = FlowKind.Sink
    val requestDebug = call("requestDebug", T4ExecutionRequest, void).async
    requestDebug.flow = FlowKind.Sink

    val preprocessingStarted = signal("preprocessingStarted", void).async
    preprocessingStarted.flow = FlowKind.Sink
    val preprocessingFinished = signal("preprocessingFinished", T4PreprocessingResult).async
    preprocessingFinished.flow = FlowKind.Sink

    call("getConfiguration", T4FileLocation, T4ConfigurationModel).async

    // Frontend calls this before executing file
    call("requestCompilation", T4FileLocation, T4BuildResult).async
    call("executionSucceeded", T4FileLocation, void).async
    call("executionFailed", T4FileLocation, void).async
    call("executionAborted", T4FileLocation, void).async

    call("getProjectDependencies", T4FileLocation, immutableList(int)).async
    // Normally, it's the backend that initiates template execution,
    // so it can perform the necessary associated data structure preparations.
    // In tests, however, this call needs to be done first
    call("prepareExecution", T4FileLocation, void)
  }
}
