package com.jetbrains.fortea.configuration.preprocess

import com.intellij.execution.configurations.RuntimeConfigurationError
import com.intellij.openapi.util.UserDataHolderBase
import com.jetbrains.fortea.configuration.readT4FileLocationExternal
import com.jetbrains.fortea.configuration.writeExternal
import com.jetbrains.rider.model.T4FileLocation
import org.jdom.Element
import java.io.File

class T4PreprocessConfigurationParameters(
  var initialFileLocation: T4FileLocation
) : UserDataHolderBase() {
  fun validate() {
    if (File(initialFileLocation.location).exists()) return
    throw RuntimeConfigurationError("Target file does not exist")
  }

  fun readExternal(element: Element) {
    initialFileLocation = readT4FileLocationExternal(element)
  }

  fun writeExternal(element: Element) = initialFileLocation.writeExternal(element)
}
