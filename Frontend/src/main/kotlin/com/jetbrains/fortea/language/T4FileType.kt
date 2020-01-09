package com.jetbrains.fortea.language

import com.intellij.openapi.fileTypes.FileTypeEditorHighlighterProviders
import com.intellij.openapi.fileTypes.TemplateLanguageFileType
import com.jetbrains.fortea.highlighting.T4EditorSyntaxHighlighter
import com.jetbrains.fortea.icons.T4Icons
import com.jetbrains.rider.ideaInterop.fileTypes.RiderLanguageFileTypeBase
import javax.swing.Icon

object T4FileType : RiderLanguageFileTypeBase(T4Language), TemplateLanguageFileType {
  init {
    FileTypeEditorHighlighterProviders.INSTANCE.addExplicitExtension(this) { project, _, virtualFile, colors ->
      T4EditorSyntaxHighlighter(project, virtualFile, colors)
    }
  }

  override fun getDefaultExtension() = "tt"
  override fun getDescription() = "T4 template"
  override fun getIcon(): Icon = T4Icons.T4
  override fun getName() = "T4"
}
