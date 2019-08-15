package model

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.SolutionModel

@Suppress("unused")
object T4SubprocessProtocolRoot : Root()

object T4SubprocessProtocolModel : Ext(T4SubprocessProtocolRoot) {
  init {
    call("resolveAssembly", string, string)
    call("resolvePath", string, string)
  }
}
