package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.colors.EditorColorsScheme
import com.intellij.openapi.editor.ex.EditorEx
import com.intellij.openapi.editor.ex.util.LayerDescriptor
import com.intellij.openapi.editor.ex.util.LayeredLexerEditorHighlighter
import com.intellij.openapi.fileEditor.FileDocumentManager
import com.intellij.openapi.fileEditor.impl.text.EditorHighlighterUpdater
import com.intellij.openapi.fileTypes.FileTypeRegistry
import com.intellij.openapi.fileTypes.SyntaxHighlighterFactory
import com.intellij.openapi.project.Project
import com.intellij.openapi.rd.createNestedDisposable
import com.intellij.openapi.util.Key
import com.intellij.openapi.vfs.VirtualFile
import com.jetbrains.fortea.highlighting.T4SyntaxHighlightingHost.Companion.getT4EditableEntityModel
import com.jetbrains.fortea.psi.T4ElementTypes
import com.jetbrains.rd.platform.util.lifetime
import com.jetbrains.rider.document.getFirstEditor
import com.jetbrains.rider.ideaInterop.fileTypes.csharp.CSharpFileType

class T4EditorSyntaxHighlighter(
  private val project: Project,
  private val virtualFile: VirtualFile,
  colors: EditorColorsScheme
) : LayeredLexerEditorHighlighter(T4SyntaxHighlighter, colors) {
  private val highlighterLifetime = project.lifetime.createNested()
  private val rawTextLayerExtension = virtualFile.t4OutputExtension

  init {
    registerLayers()
    subscribeToOutputExtensionUpdates()
  }

  private fun registerLayers() {
    val codeHighlighter = SyntaxHighlighterFactory.getSyntaxHighlighter(CSharpFileType, project, virtualFile)!!
    registerLayer(T4ElementTypes.RAW_CODE, LayerDescriptor(codeHighlighter, ""))
    if (rawTextLayerExtension == null) return
    val rawTextFileType = FileTypeRegistry.getInstance().getFileTypeByFileName("dummy.$rawTextLayerExtension")
    val rawTextHighlighter = SyntaxHighlighterFactory
      .getSyntaxHighlighter(rawTextFileType, project, virtualFile)
      ?: return
    registerLayer(T4ElementTypes.RAW_TEXT, LayerDescriptor(rawTextHighlighter, "\n"))
  }

  private fun subscribeToOutputExtensionUpdates() {
    // Since we're in a highlighting a document, it most likely is opened in the editor and has a document
    val document = FileDocumentManager.getInstance().getDocument(virtualFile) ?: return

    val editor = document.getFirstEditor(project)
    if (editor !is EditorEx) return
    val highlighterDisposable = highlighterLifetime.lifetime.createNestedDisposable()
    // Cannot highlight the text yet. We'll reset the layers
    // as soon as we know how to highlight them.
    //
    // project.lifetime is quite a long-lived lifetime,
    // but it's not a big deal here, because the editable entity
    // gets disposed properly anyway
    document.getT4EditableEntityModel(project)?.rawTextExtension?.advise(highlighterLifetime) { extension ->
      if (extension == rawTextLayerExtension) return@advise
      virtualFile.t4OutputExtension = extension
      val updater = EditorHighlighterUpdater(project, highlighterDisposable, editor, virtualFile)
      updater.updateHighlighters()
      highlighterLifetime.terminate(true)
    }
  }

  companion object {
    private val OUTPUT_EXTENSION_KEY = Key<String>("OUTPUT_EXTENSION_KEY")

    private var VirtualFile.t4OutputExtension
      get() = getUserData(OUTPUT_EXTENSION_KEY)
      set(value) = putUserData(OUTPUT_EXTENSION_KEY, value)
  }
}
