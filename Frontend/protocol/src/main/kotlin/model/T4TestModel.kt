package model

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.IdeRoot
import model.T4ProtocolModel.T4FileLocation

object T4TestModel : Ext(IdeRoot) {
  init {
    call("preprocessFile", T4FileLocation, void)
  }
}