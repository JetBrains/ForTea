package com.jetbrains.fortea.psi

import com.intellij.psi.TokenType
import com.intellij.psi.tree.IElementType

object T4TokenNodeTypes : T4ElementTypes {
  @JvmField val BAD_TOKEN: IElementType = TokenType.BAD_CHARACTER
  @JvmField val WHITE_SPACE: IElementType = TokenType.WHITE_SPACE
}