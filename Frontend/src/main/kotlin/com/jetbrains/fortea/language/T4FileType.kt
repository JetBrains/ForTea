package com.jetbrains.fortea.language

import com.jetbrains.fortea.icons.T4Icons
import com.jetbrains.rider.ideaInterop.fileTypes.RiderLanguageFileTypeBase
import javax.swing.Icon

object T4FileType : RiderLanguageFileTypeBase(T4Language) {
  override fun getDefaultExtension() = "T4"
  override fun getDescription() = "T4 template file"
  override fun getIcon(): Icon = T4Icons.T4
  override fun getName() = "T4"
}
