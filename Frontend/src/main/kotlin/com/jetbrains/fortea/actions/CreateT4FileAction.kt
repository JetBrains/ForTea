package com.jetbrains.fortea.actions

import com.intellij.ide.actions.CreateFileFromTemplateAction
import com.intellij.ide.actions.CreateFileFromTemplateDialog
import com.intellij.openapi.project.DumbAware
import com.intellij.openapi.project.Project
import com.intellij.psi.PsiDirectory
import com.intellij.psi.PsiFile
import com.jetbrains.fortea.language.T4FileType


class CreateT4FileAction : CreateFileFromTemplateAction(
  "T4 File",
  "Creates a new T4 file",
  T4FileType.icon
), DumbAware {
  override fun buildDialog(project: Project, directory: PsiDirectory, builder: CreateFileFromTemplateDialog.Builder) {
    builder.setTitle("New T4 File")
      .addKind("T4 File", T4FileType.icon, T4_TEMPLATE_NAME)
      .addKind("T4 Include", T4FileType.icon, T4_INCLUDE_NAME)
  }

  override fun getActionName(directory: PsiDirectory, newName: String, templateName: String): String {
    return "Create T4 file $newName"
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
