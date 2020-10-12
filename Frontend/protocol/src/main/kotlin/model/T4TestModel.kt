package model

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.IdeRoot
import model.T4ProtocolModel.T4FileLocation
import com.jetbrains.rd.generator.nova.csharp.CSharp50Generator
import com.jetbrains.rd.generator.nova.kotlin.Kotlin11Generator

object T4TestModel : Ext(IdeRoot) {
  init {
    setting(Kotlin11Generator.Namespace, "com.jetbrains.fortea.model")
    setting(CSharp50Generator.Namespace, "JetBrains.ForTea.RiderPlugin.Model")

    call("preprocessFile", T4FileLocation, void)
    call("waitForIndirectInvalidation", void, void)
  }
}