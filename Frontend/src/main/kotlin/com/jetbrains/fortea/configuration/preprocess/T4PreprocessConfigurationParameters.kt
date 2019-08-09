package com.jetbrains.fortea.configuration.preprocess

import com.intellij.execution.configurations.RuntimeConfigurationError
import com.intellij.openapi.util.JDOMExternalizerUtil
import com.intellij.openapi.util.UserDataHolderBase
import org.jdom.Element
import java.io.File

class T4PreprocessConfigurationParameters(
  var initialFilePath: String
) : UserDataHolderBase() {
  fun validate() {
    if (File(initialFilePath).exists()) return
    throw RuntimeConfigurationError("Target file does not exist")
  }

  fun readExternal(element: Element) {
    initialFilePath = JDOMExternalizerUtil.readField(element, INITIAL_FILE_PATH) ?: ""
  }

  fun writeExternal(element: Element) {
    JDOMExternalizerUtil.writeField(element, INITIAL_FILE_PATH, initialFilePath)
  }

  private companion object {
    private const val INITIAL_FILE_PATH = "INITIAL_FILE_PATH"
  }
}
