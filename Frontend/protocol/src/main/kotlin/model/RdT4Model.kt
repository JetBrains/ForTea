package model

import com.jetbrains.rider.model.nova.ide.SolutionModel
import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*

@Suppress("unused")
object T4ProtocolModel : Ext(SolutionModel.Solution) {
  val T4ConfigurationModel = structdef {
    field("executablePath", string)
    field("outputPath", string)
  }

  init {
    map("configurations", string, T4ConfigurationModel).async
    // Returns whether or not compilation succeeded
    call("requestCompilation", string, bool).async
    call("transferResults", string, void).async
    // returns whether or note preprocessing succeeded
    call("requestPreprocessing", string, bool)
  }
}
