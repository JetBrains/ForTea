package com.jetbrains.fortea.highlighting

import com.intellij.openapi.editor.XmlHighlighterColors
import com.intellij.openapi.editor.colors.TextAttributesKey
import com.intellij.openapi.editor.colors.TextAttributesKey.createTextAttributesKey
import com.intellij.openapi.fileTypes.SyntaxHighlighterBase
import com.intellij.psi.tree.IElementType
import com.jetbrains.fortea.lexer.T4Lexer
import com.jetbrains.fortea.psi.T4ElementTypes

object T4SyntaxHighlighter : SyntaxHighlighterBase() {
  private val BLOCK_MARKER_KEYS = arrayOf(T4TextAttributeKeys.T4_BLOCK_MARKER)
  private val EQUAL_KEYS = arrayOf(T4TextAttributeKeys.T4_DIRECTIVE_EQ_SIGN)
  private val QUOTE_KEYS = arrayOf(T4TextAttributeKeys.T4_DIRECTIVE_QUOTE)

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
