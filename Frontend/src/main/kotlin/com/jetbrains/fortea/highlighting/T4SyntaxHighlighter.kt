package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.DefaultLanguageHighlighterColors
import com.intellij.openapi.editor.XmlHighlighterColors
import com.intellij.openapi.editor.colors.TextAttributesKey
import com.intellij.openapi.editor.colors.TextAttributesKey.createTextAttributesKey
import com.intellij.openapi.fileTypes.SyntaxHighlighterBase
import com.intellij.psi.tree.IElementType
import com.jetbrains.fortea.lexer.T4Lexer
import com.jetbrains.fortea.psi.T4ElementTypes

object T4SyntaxHighlighter : SyntaxHighlighterBase() {
  val BLOCK_MARKER = createTextAttributesKey("T4_BLOCK_MARKER", XmlHighlighterColors.XML_TAG)
  val EQUAL = createTextAttributesKey("T4_DIRECTIVE_EQ_SIGN", XmlHighlighterColors.XML_ATTRIBUTE_VALUE)
  val QUOTE = createTextAttributesKey("T4_DIRECTIVE_QUOTE", XmlHighlighterColors.XML_ATTRIBUTE_VALUE)

  private val BLOCK_MARKER_KEYS = arrayOf(BLOCK_MARKER)
  private val EQUAL_KEYS = arrayOf(EQUAL)
  private val QUOTE_KEYS = arrayOf(QUOTE)

  override fun getTokenHighlights(elementType: IElementType?): Array<TextAttributesKey> = when (elementType) {
    T4ElementTypes.BLOCK_END -> BLOCK_MARKER_KEYS
    T4ElementTypes.STATEMENT_BLOCK_START -> BLOCK_MARKER_KEYS
    T4ElementTypes.DIRECTIVE_START -> BLOCK_MARKER_KEYS
    T4ElementTypes.EQUAL -> EQUAL_KEYS
    T4ElementTypes.EXPRESSION_BLOCK_START -> BLOCK_MARKER_KEYS
    T4ElementTypes.FEATURE_BLOCK_START -> BLOCK_MARKER_KEYS
    T4ElementTypes.QUOTE -> QUOTE_KEYS
    else -> TextAttributesKey.EMPTY_ARRAY
  }

  override fun getHighlightingLexer() = T4Lexer()
}
