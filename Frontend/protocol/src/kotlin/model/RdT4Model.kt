package model

import com.jetbrains.rider.model.nova.ide.SolutionModel
import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*

@Suppress("unused")
object T4ProtocolModel : Ext(SolutionModel.Solution) {
  val T4ConfigurationModel = structdef {
    field("executablePath", string)
    field("outputName", string)
  }

  init {
    map("configurations", string, T4ConfigurationModel).async
  }
}
