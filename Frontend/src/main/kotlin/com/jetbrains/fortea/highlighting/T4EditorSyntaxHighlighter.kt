package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.colors.EditorColorsScheme
import com.intellij.openapi.editor.ex.util.LayerDescriptor
import com.intellij.openapi.editor.ex.util.LayeredLexerEditorHighlighter
import com.intellij.openapi.fileEditor.FileDocumentManager
import com.intellij.openapi.fileTypes.FileTypeRegistry
import com.intellij.openapi.fileTypes.SyntaxHighlighterFactory
import com.intellij.openapi.project.Project
import com.intellij.openapi.vfs.VirtualFile
import com.jetbrains.fortea.highlighting.T4ProtocolRawTextHighlightingExtension.Companion.T4_MARKUP_MODEL_EXTENSION_KEY
import com.jetbrains.fortea.psi.T4ElementTypes
import com.jetbrains.rd.framework.RdTaskResult
import com.jetbrains.rd.platform.util.lifetime
import com.jetbrains.rdclient.daemon.components.FrontendMarkupHost
import com.jetbrains.rider.ideaInterop.fileTypes.csharp.CSharpFileType
import com.jetbrains.rider.model.T4MarkupModelExtension

class T4EditorSyntaxHighlighter(
  private val project: Project?,
  private val virtualFile: VirtualFile?,
  colors: EditorColorsScheme
) : LayeredLexerEditorHighlighter(T4SyntaxHighlighter, colors) {
  private var t4MarkupModel: T4MarkupModelExtension? = null
  private var callStamp: Int = 0
  private var isResettingText: Boolean = false
  private var oldExtension: String? = null

  init {
    val codeHighlighter = SyntaxHighlighterFactory.getSyntaxHighlighter(CSharpFileType, project, virtualFile)!!
    registerLayer(T4ElementTypes.RAW_CODE, LayerDescriptor(codeHighlighter, ""))

    if (virtualFile != null) {
      if (project != null) {
        // Since we're in a highlighting a document, it most likely is opened in the editor and has a document
        val document = FileDocumentManager.getInstance().getDocument(virtualFile)
        if (document != null) {
          val markupContributor = FrontendMarkupHost.getMarkupContributor(project, document)
          val adapter = markupContributor?.markupAdapter
          t4MarkupModel = adapter?.getExtension(T4_MARKUP_MODEL_EXTENSION_KEY)
        }
      }
    }

    // Cannot highlight the text yet. We'll reset the layers as soon as we know how to highlight them
    updateLayers()
  }

  private fun updateRawTextLayer(project: Project?, virtualFile: VirtualFile?, extension: String?) {
    if (extension == oldExtension) return
    oldExtension = extension
    if (extension == null) {
      unregisterLayer(T4ElementTypes.RAW_TEXT)
      return
    }
    val rawTextFileType = FileTypeRegistry.getInstance().getFileTypeByFileName("dummy.$extension")
    val rawTextHighlighter = SyntaxHighlighterFactory
      .getSyntaxHighlighter(rawTextFileType, project, virtualFile) ?: return
    registerLayer(T4ElementTypes.RAW_TEXT, LayerDescriptor(rawTextHighlighter, "\n"))
    synchronized(this) {
      isResettingText = true
      try {
        val text = myText
        myText = null
        setText(text)
      } finally {
        isResettingText = false
      }
    }
  }

  override fun updateLayers(): Boolean {
    if (isResettingText) return false
    callStamp += 1
    val currentCallStamp = callStamp
    if (project == null) return false
    // project.lifetime is quite a long-lived lifetime,
    // but it's not a big deal here, because the markup model
    // gets disposed as soon as the editor closes anyway
    t4MarkupModel?.rawTextExtension?.start(project.lifetime, Unit)?.result?.view(project.lifetime) { _, result ->
      val extension = if (result is RdTaskResult.Success) {
        result.value
      } else null
      if (currentCallStamp != callStamp) return@view
      updateRawTextLayer(project, virtualFile, extension)
    }
    return false
  }
}
