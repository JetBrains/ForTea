package com.jetbrains.fortea.utils

import com.jetbrains.rd.framework.IRdCall
import com.jetbrains.rider.util.idea.application

fun <TReq> IRdCall<TReq, Unit>.startOrSync(arg: TReq) {
  if (!application.isUnitTestMode) start(arg)
  else sync(arg)
}
