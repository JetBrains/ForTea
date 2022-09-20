package model

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rd.generator.nova.csharp.CSharp50Generator
import com.jetbrains.rd.generator.nova.kotlin.Kotlin11Generator
import com.jetbrains.rider.model.nova.ide.SolutionModel.RdDocumentModel

@Suppress("unused")
object T4RdDocumentModel : Ext(RdDocumentModel) {
  init {
    setting(Kotlin11Generator.Namespace, "com.jetbrains.fortea.model")
    setting(CSharp50Generator.Namespace, "JetBrains.ForTea.RiderPlugin.Model")

    property("rawTextExtension", string.nullable).async.doc("Extension of the output file." +
      " If the file contains output directive with extension attribute, relies on that attribute value." +
      " Returns \"cs\" for executable files without explicitly specified extension." +
      " Returns null for preprocessed files without explicitly specified extension." +
      " Does not contain dot."
    )
  }
}