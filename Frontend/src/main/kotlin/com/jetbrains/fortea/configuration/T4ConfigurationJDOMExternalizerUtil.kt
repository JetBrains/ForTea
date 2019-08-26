package com.jetbrains.fortea.configuration

import com.intellij.openapi.util.JDOMExternalizerUtil
import com.jetbrains.fortea.configuration.T4ConfigurationJDDOMExternalizerConstants.INITIAL_T4_FILE_ID
import com.jetbrains.rider.model.T4FileLocation
import org.jdom.Element

fun T4FileLocation.writeExternal(element: Element) {

  JDOMExternalizerUtil.writeField(element, INITIAL_T4_FILE_ID, id.toString())
}

fun readT4FileLocationExternal(element: Element): T4FileLocation {
  val id: Int = JDOMExternalizerUtil.readField(element, INITIAL_T4_FILE_ID)?.toIntOrNull() ?: 0
  return T4FileLocation(id)
}

object T4ConfigurationJDDOMExternalizerConstants {
  const val INITIAL_T4_FILE_ID = "INITIAL_T4_FILE_PATH"
}
