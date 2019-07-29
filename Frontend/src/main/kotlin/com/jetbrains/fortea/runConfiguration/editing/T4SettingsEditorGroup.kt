package com.jetbrains.fortea.runConfiguration.editing

import com.intellij.openapi.options.SettingsEditorGroup
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.runConfiguration.T4Configuration

class T4SettingsEditorGroup(project: Project) : SettingsEditorGroup<T4Configuration>() {
  init {
    addEditor("Configuration", T4ConfigurationEditor(project))
  }
}
