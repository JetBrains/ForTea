package com.jetbrains.fortea.inTests

import com.jetbrains.fortea.model.T4FileLocation
import com.jetbrains.fortea.model.t4TestModel
import com.jetbrains.rd.framework.IProtocol
import com.jetbrains.rdclient.util.idea.callSynchronously
import com.jetbrains.rider.inTests.InTestsConstants
import com.jetbrains.rider.protocol.ProtocolManager
import org.apache.commons.logging.LogFactory

private val frameworkLogger = LogFactory.getLog(InTestsConstants.frameworkLoggerName)!!

// TODO: fun waitForIndirectFileInvalidation() {...}

fun IProtocol.preprocessFile(t4FileLocation: T4FileLocation) {
  if (ProtocolManager.isResharperBackendDisabled()) throw IllegalStateException("ReSharper backend is disabled")
  t4TestModel.preprocessFile.callSynchronously(t4FileLocation, protocol, lifetime)
  frameworkLogger.info("Complete template preprocessing")
}

fun IProtocol.waitForIndirectInvalidation() {
  if (ProtocolManager.isResharperBackendDisabled()) throw IllegalStateException("ReSharper backend is disabled")
  frameworkLogger.info("Starting waitForIndirectInvalidation")
  t4TestModel.waitForIndirectInvalidation.callSynchronously(Unit, protocol, lifetime)
  frameworkLogger.info("Complete waitForIndirectInvalidation")
}
