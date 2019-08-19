package com.jetbrains.fortea.configuration

import com.intellij.openapi.util.JDOMExternalizerUtil
import com.jetbrains.fortea.configuration.T4ConfigurationJDDOMExternalizerConstants.INITIAL_FILE_PATH
import com.jetbrains.fortea.configuration.T4ConfigurationJDDOMExternalizerConstants.PROJECT_ID
import com.jetbrains.rider.model.T4FileLocation
import org.jdom.Element

fun T4FileLocation.writeExternal(element: Element) {
  JDOMExternalizerUtil.writeField(element, INITIAL_FILE_PATH, location)
  JDOMExternalizerUtil.writeField(element, PROJECT_ID, projectId.toString())
}

fun readT4FileLocationExternal(element: Element): T4FileLocation {
  val location = JDOMExternalizerUtil.readField(element, INITIAL_FILE_PATH) ?: ""
  val id: Int = JDOMExternalizerUtil.readField(element, PROJECT_ID)?.toIntOrNull() ?: 0
  return T4FileLocation(location, id)
}

object T4ConfigurationJDDOMExternalizerConstants {
  const val INITIAL_FILE_PATH = "INITIAL_FILE_PATH"
  const val PROJECT_ID = "PROJECT_ID" // TODO: is it persistent?
}
