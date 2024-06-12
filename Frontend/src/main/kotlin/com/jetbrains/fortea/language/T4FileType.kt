package com.jetbrains.fortea.language

import com.intellij.openapi.fileTypes.FileTypeEditorHighlighterProviders
import com.intellij.openapi.fileTypes.TemplateLanguageFileType
import com.jetbrains.fortea.highlighting.T4EditorSyntaxHighlighter
import com.jetbrains.fortea.icons.T4Icons
import com.jetbrains.fortea.utils.RiderT4Bundle
import com.jetbrains.rider.ideaInterop.fileTypes.RiderLanguageFileTypeBase
import javax.swing.Icon

object T4FileType : RiderLanguageFileTypeBase(T4Language), TemplateLanguageFileType {
  init {
    FileTypeEditorHighlighterProviders.getInstance().addExplicitExtension(this) { project, _, virtualFile, colors ->
      if (project != null && virtualFile != null) T4EditorSyntaxHighlighter(project, virtualFile, colors)
      else throw NullPointerException("T4 does not operate without Project and/or VirtualFile")
    }
  }

  override fun getDefaultExtension() = "tt"
  override fun getDescription() = RiderT4Bundle.message("label.t4.template")
  override fun getIcon(): Icon = T4Icons.T4
  override fun getName() = "T4"
}
