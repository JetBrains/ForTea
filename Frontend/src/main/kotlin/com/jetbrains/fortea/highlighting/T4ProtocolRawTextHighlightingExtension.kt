package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.Document
import com.intellij.openapi.util.Key
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rdclient.daemon.IFrontendProtocolMarkupExtension
import com.jetbrains.rider.model.MarkupModelExtension
import com.jetbrains.rider.model.T4MarkupModelExtension

class T4ProtocolRawTextHighlightingExtension : IFrontendProtocolMarkupExtension {
  override fun createExtensions(lifetime: Lifetime, document: Document): List<MarkupModelExtension> {
    return listOf(T4MarkupModelExtension(T4_MARKUP_MODEL_EXTENSION_KEY.toString()))
  }

  companion object {
    val T4_MARKUP_MODEL_EXTENSION_KEY = Key<T4MarkupModelExtension>("T4_MARKUP_MODEL_EXTENSION_KEY")
  }
}