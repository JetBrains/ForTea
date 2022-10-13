package com.jetbrains.fortea.inTests

import com.jetbrains.rdclient.protocol.IProtocolHost
import com.jetbrains.rdclient.protocol.ProtocolComponent
import com.jetbrains.rdclient.util.idea.callSynchronously
import com.jetbrains.rider.inTests.InTestsConstants
import com.jetbrains.fortea.model.T4FileLocation
import com.jetbrains.fortea.model.t4TestModel
import com.jetbrains.rider.protocol.ProtocolManager
import org.apache.commons.logging.LogFactory

class T4TestHost(protocolHost: IProtocolHost) : ProtocolComponent(protocolHost) {
  private val frameworkLogger = LogFactory.getLog(InTestsConstants.frameworkLoggerName)!!

  // TODO: fun waitForIndirectFileInvalidation() {...}

  fun preprocessFile(t4FileLocation: T4FileLocation) {
    if (ProtocolManager.isResharperBackendDisabled()) throw IllegalStateException("ReSharper backend is disabled")
    protocol.t4TestModel.preprocessFile.callSynchronously(t4FileLocation, protocol, lifetime)
    frameworkLogger.info("Complete template preprocessing")
  }

  fun waitForIndirectInvalidation() {
    if (ProtocolManager.isResharperBackendDisabled()) throw IllegalStateException("ReSharper backend is disabled")
    frameworkLogger.info("Starting waitForIndirectInvalidation")
    protocol.t4TestModel.waitForIndirectInvalidation.callSynchronously(Unit, protocol, lifetime)
    frameworkLogger.info("Complete waitForIndirectInvalidation")
  }

  companion object {
    fun getInstance(protocolHost: IProtocolHost) = protocolHost.components.filterIsInstance<T4TestHost>().single()
  }
}