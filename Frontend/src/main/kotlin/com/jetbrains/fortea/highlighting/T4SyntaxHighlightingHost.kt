package com.jetbrains.fortea.highlighting

import com.intellij.openapi.components.service
import com.intellij.openapi.editor.Document
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.Key
import com.intellij.openapi.util.getOrCreateUserData
import com.jetbrains.fortea.model.T4RdDocumentModel
import com.jetbrains.fortea.model.t4RdDocumentModel
import com.jetbrains.rd.ide.model.RdDocumentId
import com.jetbrains.rd.ide.model.RdDocumentModel
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.put
import com.jetbrains.rdclient.document.FrontendDocumentHostListener
import com.jetbrains.rdclient.util.idea.LifetimedProjectComponent

// TODO: Remove the service at all, keep only DocumentListener
class T4SyntaxHighlightingHost(project: Project) : LifetimedProjectComponent(project) {
  companion object {
    fun getInstance(project: Project): T4SyntaxHighlightingHost = project.service()

    private val T4_DOCUMENT_EDITABLE_ENTRY_KEY = Key<MutableMap<Document, T4RdDocumentModel>>("T4_DOCUMENT_EDITABLE_ENTRY_KEY")

    fun Document.getT4RdDocumentModel(project: Project) =
      project.getUserData(T4_DOCUMENT_EDITABLE_ENTRY_KEY)?.get(this)

    private fun putT4RdDocumentModel(lifetime: Lifetime, project: Project, document: Document, model: T4RdDocumentModel) {
      val map = project.getOrCreateUserData(T4_DOCUMENT_EDITABLE_ENTRY_KEY) { mutableMapOf() }
      map.put(lifetime, document, model)
    }
  }

  class DocumentListener : FrontendDocumentHostListener {
    override fun documentBound(lifetime: Lifetime,
                               project: Project,
                               documentId: RdDocumentId,
                               documentModel: RdDocumentModel,
                               document: Document) {
      putT4RdDocumentModel(lifetime, project, document, documentModel.t4RdDocumentModel)
    }
  }
}