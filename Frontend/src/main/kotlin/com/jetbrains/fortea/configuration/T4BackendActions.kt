package com.jetbrains.fortea.configuration

import com.jetbrains.rider.actions.base.RiderContextAwareAnAction
import com.jetbrains.rider.icons.ReSharperLiveTemplatesCSharpIcons

//class T4ExecuteTemplateBackendAction : RiderContextAwareAnAction("T4.RunFromContext")
//class T4DebugTemplateBackendAction : RiderContextAwareAnAction("T4.DebugFromContext")
class T4PreprocessTemplateBackendAction :
  RiderContextAwareAnAction("T4.PreprocessFromContext", icon = ReSharperLiveTemplatesCSharpIcons.ScopeCS)
