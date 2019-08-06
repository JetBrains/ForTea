package model

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.SolutionModel

@Suppress("unused")
object T4ProtocolModel : Ext(SolutionModel.Solution) {
  val messageKind = enum("T4BuildMessageKind") {
    +"T4Message"
    +"T4Success"
    +"T4Warning"
    +"T4Error"
  }

  val buildMessage = structdef {
    field("buildMessageKind", messageKind)
    field("message", string)
  }

  val resultKind = enum("T4BuildResultKind") {
    +"HasErrors"
    +"HasWarnings"
    +"Successful"
    // +"Canceled"
  }

  val buildResult = structdef("T4BuildResult") {
    field("buildResultKind", resultKind)
    field("messages", immutableList(buildMessage))
  }

  val T4ConfigurationModel = structdef {
    field("executablePath", string)
    field("outputPath", string)
  }

  init {
    map("configurations", string, T4ConfigurationModel).async
    call("requestCompilation", string, buildResult).async
    call("transferResults", string, void).async
    call("requestPreprocessing", string, bool)
  }
}
