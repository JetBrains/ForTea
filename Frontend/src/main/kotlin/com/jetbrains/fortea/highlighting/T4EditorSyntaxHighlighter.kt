package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.Document
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
import com.jetbrains.rd.platform.util.application
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
  private var rawTextLayerExtension: String? = null
  private var outputExtension: String? = null

  init {
    val codeHighlighter = SyntaxHighlighterFactory.getSyntaxHighlighter(CSharpFileType, project, virtualFile)!!
    registerLayer(T4ElementTypes.RAW_CODE, LayerDescriptor(codeHighlighter, ""))
    setUp()
  }

  private fun setUp() {
    if (virtualFile == null) return
    // Since we're in a highlighting a document, it most likely is opened in the editor and has a document
    val document = FileDocumentManager.getInstance().getDocument(virtualFile) ?: return
    if (project == null) return
    val markupContributor = FrontendMarkupHost.getMarkupContributor(project, document)
    val adapter = markupContributor?.markupAdapter
    t4MarkupModel = adapter?.getExtension(T4_MARKUP_MODEL_EXTENSION_KEY)
    // Cannot highlight the text yet. We'll reset the layers
    // as soon as we know how to highlight them
    subscribe(document)
  }

  private fun subscribe(document: Document) {
    if (project == null) return
    // project.lifetime is quite a long-lived lifetime,
    // but it's not a big deal here, because the markup model
    // gets disposed as soon as the editor closes anyway
    t4MarkupModel?.rawTextExtension?.advise(project.lifetime) { extension ->
      if (extension == outputExtension) return@advise
      outputExtension = extension
      application.invokeLater {
        synchronized(this) {
          // will trigger layer invalidation
          setText(document.text)
        }
      }
    }
  }

  override fun updateLayers(): Boolean {
    if (project == null) return false
    if (rawTextLayerExtension == outputExtension) return false
    rawTextLayerExtension = outputExtension
    val rawTextFileType = FileTypeRegistry.getInstance().getFileTypeByFileName("dummy.$rawTextLayerExtension")
    val rawTextHighlighter = SyntaxHighlighterFactory.getSyntaxHighlighter(rawTextFileType, project, virtualFile)
    if (rawTextHighlighter == null) {
      unregisterLayer(T4ElementTypes.RAW_TEXT)
      return false
    }
    registerLayer(T4ElementTypes.RAW_TEXT, LayerDescriptor(rawTextHighlighter, "\n"))
    return true
  }
}
