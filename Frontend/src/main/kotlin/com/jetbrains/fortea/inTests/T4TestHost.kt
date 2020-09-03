package com.jetbrains.fortea.inTests

import com.jetbrains.rdclient.protocol.IProtocolHost
import com.jetbrains.rdclient.protocol.ProtocolComponent
import com.jetbrains.rdclient.util.idea.callSynchronously
import com.jetbrains.rider.inTests.InTestsConstants
import com.jetbrains.rider.model.T4FileLocation
import com.jetbrains.rider.model.T4TestModel
import com.jetbrains.rider.protocol.ProtocolManager
import org.apache.commons.logging.LogFactory

class T4TestHost(protocolHost: IProtocolHost) : ProtocolComponent(protocolHost) {
  private val model = T4TestModel.create(lifetime, protocol)
  private val frameworkLogger = LogFactory.getLog(InTestsConstants.frameworkLoggerName)!!

  // TODO: fun waitForIndirectFileInvalidation() {...}

  fun preprocessFile(t4FileLocation: T4FileLocation) {
    if (ProtocolManager.isResharperBackendDisabled()) throw IllegalStateException("ReSharper backend is disabled")
    model.preprocessFile.callSynchronously(t4FileLocation, protocol, lifetime)
    frameworkLogger.info("Complete template preprocessing")
  }

  companion object {
    fun getInstance(protocolHost: IProtocolHost) = protocolHost.components.filterIsInstance<T4TestHost>().single()
  }
}