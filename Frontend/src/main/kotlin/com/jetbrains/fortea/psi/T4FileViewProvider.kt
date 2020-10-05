package com.jetbrains.fortea.psi

import com.intellij.lang.Language
import com.intellij.lang.LanguageParserDefinitions
import com.intellij.openapi.vfs.VirtualFile
import com.intellij.psi.MultiplePsiFilesPerDocumentFileViewProvider
import com.intellij.psi.PsiFile
import com.intellij.psi.PsiManager
import com.intellij.psi.impl.source.PsiFileImpl
import com.intellij.psi.templateLanguages.TemplateLanguageFileViewProvider
import com.jetbrains.rider.ideaInterop.fileTypes.csharp.CSharpLanguage

class T4FileViewProvider(
  file: VirtualFile,
  manager: PsiManager,
  physical: Boolean,
  private val language: Language
) : MultiplePsiFilesPerDocumentFileViewProvider(
  manager,
  file,
  physical
), TemplateLanguageFileViewProvider {
  private val languages = hashSetOf(language, CSharpLanguage)

  override fun getBaseLanguage() = language
  override fun getLanguages() = languages
  override fun cloneInner(fileCopy: VirtualFile) = T4FileViewProvider(fileCopy, manager, false, language)
  override fun getTemplateDataLanguage()= CSharpLanguage

  override fun createFile(lang: Language): PsiFile? {
    if (lang == CSharpLanguage) {
      val file = LanguageParserDefinitions.INSTANCE.forLanguage(lang)!!.createFile(this) as PsiFileImpl
      file.contentElementType = T4FileElementTypes.T4_CODE_DATA
      return file
    }

    if (lang === baseLanguage) {
      return LanguageParserDefinitions.INSTANCE.forLanguage(lang)!!.createFile(this)
    }

    return null
  }
}