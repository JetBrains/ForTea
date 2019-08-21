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

  val T4PreprocessingResult = structdef {
    field("succeeded", bool)
    field("message", T4BuildMessage.nullable)
  }

  val T4ConfigurationModel = structdef {
    field("executablePath", string)
    field("outputPath", string)
  }

  val T4FileLocation = structdef {
    field("location", string)
    field("projectId", int)
  }

  init {
    property("userSessionActive", bool)
    call("getConfiguration", T4FileLocation, T4ConfigurationModel).async
    call("requestCompilation", T4FileLocation, T4BuildResult).async
    call("executionSucceeded", T4FileLocation, void).async
    call("requestPreprocessing", T4FileLocation, T4PreprocessingResult).async
  }
}
