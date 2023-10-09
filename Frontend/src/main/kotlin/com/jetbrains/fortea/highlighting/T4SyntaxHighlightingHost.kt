package com.jetbrains.fortea.highlighting

import com.intellij.openapi.components.service
import com.intellij.openapi.editor.Document
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.Key
import com.intellij.openapi.util.getOrCreateUserData
import com.jetbrains.fortea.model.T4RdDocumentModel
import com.jetbrains.fortea.model.t4RdDocumentModel
import com.jetbrains.rd.framework.IProtocol
import com.jetbrains.rd.ide.model.RdDocumentId
import com.jetbrains.rd.ide.model.RdDocumentModel
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.put
import com.jetbrains.rdclient.document.FrontendDocumentHostListener
import com.jetbrains.rider.protocol.toProject

class T4SyntaxHighlightingHost {
  companion object {

    private val T4_DOCUMENT_EDITABLE_ENTRY_KEY = Key<MutableMap<Document, T4RdDocumentModel>>("T4_DOCUMENT_EDITABLE_ENTRY_KEY")

    fun Document.getT4RdDocumentModel(project: Project) =
      project.getUserData(T4_DOCUMENT_EDITABLE_ENTRY_KEY)?.get(this)

    private fun putT4RdDocumentModel(lifetime: Lifetime, protocol: IProtocol, document: Document, model: T4RdDocumentModel) {
      val map = protocol.toProject.getOrCreateUserData(T4_DOCUMENT_EDITABLE_ENTRY_KEY) { mutableMapOf() }
      map.put(lifetime, document, model)
    }
  }

  class DocumentListener : FrontendDocumentHostListener {
    override fun documentBound(lifetime: Lifetime,
                               protocol: IProtocol,
                               documentId: RdDocumentId,
                               documentModel: RdDocumentModel,
                               document: Document) {
      putT4RdDocumentModel(lifetime, protocol, document, documentModel.t4RdDocumentModel)
    }
  }
}