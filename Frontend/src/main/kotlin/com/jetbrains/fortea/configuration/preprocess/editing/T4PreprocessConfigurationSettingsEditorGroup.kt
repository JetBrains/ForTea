package com.jetbrains.fortea.configuration.preprocess.editing

import com.intellij.openapi.options.SettingsEditorGroup
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfiguration

class T4PreprocessConfigurationSettingsEditorGroup(project: Project) :
  SettingsEditorGroup<T4PreprocessConfiguration>() {
  init {
    addEditor("Configuration", T4PreprocessConfigurationEditor(project))
  }
}
