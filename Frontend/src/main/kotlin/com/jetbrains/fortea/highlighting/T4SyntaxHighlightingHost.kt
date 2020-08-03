package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.Document
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.Key
import com.jetbrains.rd.platform.util.idea.getOrCreateUserData
import com.jetbrains.rd.util.addUnique
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rdclient.document.FrontendDocumentHost
import com.jetbrains.rdclient.util.idea.LifetimedProjectComponent
import com.jetbrains.rider.model.T4EditableEntityModel
import com.jetbrains.rider.model.t4EditableEntityModel

class T4SyntaxHighlightingHost(project: Project) : LifetimedProjectComponent(project) {
  companion object {
    private val T4_DOCUMENT_EDITABLE_ENTRY_KEY = Key<MutableMap<Document, T4EditableEntityModel>>("T4_DOCUMENT_EDITABLE_ENTRY_KEY")

    fun Document.getT4EditableEntityModel(project: Project) =
      project.getUserData(T4_DOCUMENT_EDITABLE_ENTRY_KEY)?.get(this)

    private fun putT4EditableEntityModel(lifetime: Lifetime, project: Project, document: Document, model: T4EditableEntityModel) {
      val map = project.getOrCreateUserData(T4_DOCUMENT_EDITABLE_ENTRY_KEY) { mutableMapOf() }
      map.addUnique(lifetime, document, model)
    }
  }

  init {
    val documentHost = FrontendDocumentHost.getInstance(project)
    documentHost.openedDocuments.view(componentLifetime) { lifetime, entityId, document ->
      val protocolEditableEntity = documentHost.getEditableEntityOrThrow(entityId)
      putT4EditableEntityModel(lifetime, project, document, protocolEditableEntity.t4EditableEntityModel)
    }
  }
}