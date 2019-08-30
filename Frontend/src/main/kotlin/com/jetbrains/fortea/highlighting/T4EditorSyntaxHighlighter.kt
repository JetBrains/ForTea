package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.colors.EditorColorsScheme
import com.intellij.openapi.editor.ex.util.LayerDescriptor
import com.intellij.openapi.editor.ex.util.LayeredLexerEditorHighlighter
import com.intellij.openapi.fileTypes.SyntaxHighlighterFactory
import com.intellij.openapi.project.Project
import com.intellij.openapi.vfs.VirtualFile
import com.jetbrains.fortea.psi.T4ElementTypes
import com.jetbrains.rider.ideaInterop.fileTypes.csharp.CSharpFileType

class T4EditorSyntaxHighlighter(
  project: Project?,
  virtualFile: VirtualFile?,
  colors: EditorColorsScheme
) : LayeredLexerEditorHighlighter(T4SyntaxHighlighter, colors) {
  init {
    val codeHighlighter = SyntaxHighlighterFactory.getSyntaxHighlighter(CSharpFileType, project, virtualFile)!!
    registerLayer(T4ElementTypes.RAW_CODE, LayerDescriptor(codeHighlighter, ""))
  }
}
