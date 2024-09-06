package com.jetbrains.fortea.highlighting

import com.intellij.openapi.client.ClientAppSession
import com.intellij.openapi.diagnostic.logger
import com.intellij.openapi.editor.Document
import com.intellij.openapi.editor.Editor
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.Key
import com.intellij.openapi.util.getOrCreateUserData
import com.intellij.openapi.util.getOrCreateUserDataUnsafe
import com.jetbrains.fortea.model.T4RdDocumentModel
import com.jetbrains.fortea.model.t4RdDocumentModel
import com.jetbrains.rd.framework.IProtocol
import com.jetbrains.rd.ide.model.TextControlId
import com.jetbrains.rd.ide.model.TextControlModel
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.put
import com.jetbrains.rdclient.client.protocol
import com.jetbrains.rdclient.document.FrontendDocumentHost
import com.jetbrains.rdclient.document.getDocumentId
import com.jetbrains.rdclient.editors.FrontendTextControlHostListener
import com.jetbrains.rider.protocol.toProject

class T4SyntaxHighlightingHost {
  companion object {

    private val T4_DOCUMENT_EDITABLE_ENTRY_KEY = Key<MutableMap<Document, T4RdDocumentModel>>("T4_DOCUMENT_EDITABLE_ENTRY_KEY")

    fun Document.getT4RdDocumentModel(project: Project) =
      project.getUserData(T4_DOCUMENT_EDITABLE_ENTRY_KEY)?.get(this)

    private fun putT4RdDocumentModel(lifetime: Lifetime, protocol: IProtocol, document: Document, model: T4RdDocumentModel) {
      val map = protocol.toProject.getOrCreateUserDataUnsafe(T4_DOCUMENT_EDITABLE_ENTRY_KEY) { mutableMapOf() }
      map.put(lifetime, document, model)
    }
  }

  class TextControlHostListener : FrontendTextControlHostListener {
    override fun editorBound(
      lifetime: Lifetime,
      appSession: ClientAppSession,
      textControlId: TextControlId,
      editorModel: TextControlModel,
      editor: Editor,
    ) {
      val documentId = editor.document.getDocumentId(appSession)
      if (documentId != null) {
        val synchronizer = FrontendDocumentHost.getInstance(appSession).getSynchronizer(documentId)
        if (synchronizer == null) {
          logger<TextControlHostListener>().error("Failed to acquire a document synchronizer. T4 protocol initialization failed")
          return
        }
        putT4RdDocumentModel(lifetime, appSession.protocol, editor.document, synchronizer.modelDocument.t4RdDocumentModel)
      }
    }
  }
}