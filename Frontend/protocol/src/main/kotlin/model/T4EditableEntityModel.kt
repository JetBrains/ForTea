package model

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.SolutionModel
import com.jetbrains.rider.model.nova.ide.SolutionModel.EditableEntity

object T4EditableEntityModel : Ext(EditableEntity) {
  init {
    // todo: do we need it to be async?
    property("rawTextExtension", string.nullable).async.doc("Extension of the output file." +
      " If the file contains output directive with extension attribute, relies on that attribute value." +
      " Returns \"cs\" for executable files without explicitly specified extension." +
      " Returns null for preprocessed files without explicitly specified extension." +
      " Does not contain dot."
    )
  }
}