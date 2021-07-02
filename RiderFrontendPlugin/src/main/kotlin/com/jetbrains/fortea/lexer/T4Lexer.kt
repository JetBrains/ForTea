package com.jetbrains.fortea.lexer

import com.intellij.lexer.FlexAdapter
import com.intellij.lexer.MergingLexerAdapter
import com.intellij.psi.tree.TokenSet
import com.jetbrains.fortea.psi.T4ElementTypes

class T4Lexer : MergingLexerAdapter(FlexAdapter(_T4Lexer(null)), tokensToMerge) {
  companion object {
    private val tokensToMerge = TokenSet.create(
      T4ElementTypes.RAW_ATTRIBUTE_VALUE,
      T4ElementTypes.RAW_CODE,
      T4ElementTypes.RAW_TEXT
    )
  }
}
