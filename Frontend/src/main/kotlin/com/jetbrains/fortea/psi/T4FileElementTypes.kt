package com.jetbrains.fortea.psi

import com.intellij.psi.tree.IFileElementType
import com.intellij.psi.tree.OuterLanguageElementType
import com.jetbrains.fortea.language.T4Language

object T4FileElementTypes {
  val FILE = IFileElementType(T4Language)
  val T4_OUTER_TYPE = OuterLanguageElementType("T4_FRAGMENT", T4Language)
  val T4_CODE_DATA = T4CodeDataElementType()
}