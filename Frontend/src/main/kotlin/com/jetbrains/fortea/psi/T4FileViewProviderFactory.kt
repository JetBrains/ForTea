package com.jetbrains.fortea.psi

import com.intellij.lang.Language
import com.intellij.openapi.vfs.VirtualFile
import com.intellij.psi.FileViewProviderFactory
import com.intellij.psi.PsiManager

class T4FileViewProviderFactory : FileViewProviderFactory {
  override fun createFileViewProvider(
    file: VirtualFile,
    language: Language,
    manager: PsiManager,
    eventSystemEnabled: Boolean
  ) = T4FileViewProvider(file, manager, eventSystemEnabled, language)
}