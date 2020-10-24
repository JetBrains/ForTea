package com.jetbrains.fortea.psi

import com.intellij.psi.templateLanguages.TemplateDataElementType
import com.jetbrains.fortea.language.T4Language

class T4CodeDataElementType : TemplateDataElementType(
  "T4_CODE_DATA",
  T4Language,
  T4ElementTypes.RAW_CODE,
  T4FileElementTypes.T4_OUTER_TYPE
)