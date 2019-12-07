package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.XmlHighlighterColors
import com.jetbrains.rider.colors.IRiderTextAttributeKeys

object T4TextAttributeKeys : IRiderTextAttributeKeys {
  val T4_BLOCK_MARKER = key("T4_BLOCK_MARKER", XmlHighlighterColors.XML_TAG)
  val T4_DIRECTIVE_EQ_SIGN = key("T4_DIRECTIVE_EQ_SIGN", XmlHighlighterColors.XML_ATTRIBUTE_VALUE)
  val T4_DIRECTIVE_QUOTE = key("T4_DIRECTIVE_QUOTE", XmlHighlighterColors.XML_ATTRIBUTE_VALUE)
}