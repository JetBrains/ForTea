package com.jetbrains.fortea.highlighting

import com.intellij.openapi.client.ClientAppSession
import com.intellij.openapi.diagnostic.logger
import com.intellij.openapi.editor.Document
import com.intellij.openapi.editor.Editor
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.Key
import com.intellij.openapi.util.getOrCreateUserDataUnsafe
import com.jetbrains.fortea.model.T4RdDocumentModel
import com.jetbrains.fortea.model.t4RdDocumentModel
import com.jetbrains.rd.ide.model.RdDocumentId
import com.jetbrains.rd.ide.model.RdDocumentModel
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.put
import com.jetbrains.rdclient.client.frontendProjectSession
import com.jetbrains.rdclient.document.FrontendDocumentHostListener

class T4SyntaxHighlightingHost {
  companion object {

    private val T4_DOCUMENT_EDITABLE_ENTRY_KEY = Key<MutableMap<Document, T4RdDocumentModel>>("T4_DOCUMENT_EDITABLE_ENTRY_KEY")

    fun Document.getT4RdDocumentModel(project: Project) =
      project.frontendProjectSession.appSession.getUserData(T4_DOCUMENT_EDITABLE_ENTRY_KEY)?.get(this)

    private fun putT4RdDocumentModel(lifetime: Lifetime, session: ClientAppSession, document: Document, model: T4RdDocumentModel) {
      val map = session.getOrCreateUserDataUnsafe(T4_DOCUMENT_EDITABLE_ENTRY_KEY) { mutableMapOf() }
      map.put(lifetime, document, model)
    }
  }

  class TextControlHostListener : FrontendDocumentHostListener {
    override fun documentBound(
      lifetime: Lifetime,
      session: ClientAppSession,
      documentId: RdDocumentId,
      documentModel: RdDocumentModel,
      document: Document,
    ) {
      putT4RdDocumentModel(lifetime, session, document, documentModel.t4RdDocumentModel)
    }
  }
}