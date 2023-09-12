package com.jetbrains.fortea.inTests

import com.intellij.util.application
import com.jetbrains.rider.protocol.IProtocolHost
import com.jetbrains.rider.protocol.ProtocolComponentFactory
import com.jetbrains.rider.inTests.isPlayBackTestMode

class T4TestHostFactory : ProtocolComponentFactory {
  override fun create(protocolHost: IProtocolHost) =
    if (application.isUnitTestMode || application.isPlayBackTestMode) T4TestHost(protocolHost)
    else null
}