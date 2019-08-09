package com.jetbrains.fortea.configuration.run.editing

import com.intellij.openapi.options.SettingsEditorGroup
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.run.T4RunConfiguration

class T4RunConfigurationSettingsEditorGroup(project: Project) : SettingsEditorGroup<T4RunConfiguration>() {
  init {
    addEditor("Configuration", T4RunConfigurationEditor(project))
  }
}
