package com.jetbrains.fortea.actions

import com.intellij.ide.actions.CreateFileFromTemplateAction
import com.intellij.ide.actions.CreateFileFromTemplateDialog
import com.intellij.openapi.project.DumbAware
import com.intellij.openapi.project.Project
import com.intellij.psi.PsiDirectory
import com.intellij.psi.PsiFile
import com.jetbrains.fortea.language.T4FileType
import com.jetbrains.fortea.utils.RiderT4Bundle


class CreateT4FileAction : CreateFileFromTemplateAction(
  RiderT4Bundle.message("action.t4.file.create.title"),
  RiderT4Bundle.message("action.t4.file.create.description"),
  T4FileType.icon
), DumbAware {
  override fun buildDialog(project: Project, directory: PsiDirectory, builder: CreateFileFromTemplateDialog.Builder) {
    builder.setTitle(RiderT4Bundle.message("dialog.title.new.t4.file"))
      .addKind(RiderT4Bundle.message("list.item.t4.file"), T4FileType.icon, T4_TEMPLATE_NAME)
      .addKind(RiderT4Bundle.message("list.item.t4.include"), T4FileType.icon, T4_INCLUDE_NAME)
  }

  override fun getActionName(directory: PsiDirectory, newName: String, templateName: String): String {
    return RiderT4Bundle.message("command.name.create.t4.file", newName)
  }

  override fun createFile(name: String, templateName: String, dir: PsiDirectory): PsiFile? {
    val newTemplateName = when {
      name.endsWith(".ttinclude") -> T4_INCLUDE_NAME
      name.endsWith(".tt") || name.endsWith(".tt") -> T4_TEMPLATE_NAME
      else -> templateName
    }
    return super.createFile(name, newTemplateName, dir)
  }

  companion object {
    const val T4_TEMPLATE_NAME = "T4 File"
    const val T4_INCLUDE_NAME = "T4 Include"
  }
}
