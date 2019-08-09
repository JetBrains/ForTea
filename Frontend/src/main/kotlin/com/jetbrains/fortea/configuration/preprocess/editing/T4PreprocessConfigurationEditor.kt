package com.jetbrains.fortea.configuration.preprocess.editing

import com.intellij.openapi.options.SettingsEditor
import com.intellij.openapi.project.Project
import com.jetbrains.fortea.configuration.preprocess.T4PreprocessConfiguration
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.lifetime.SequentialLifetimes
import com.jetbrains.rider.run.configurations.controls.ControlViewBuilder
import javax.swing.JComponent

class T4PreprocessConfigurationEditor(private val project: Project) : SettingsEditor<T4PreprocessConfiguration>() {
  private lateinit var viewModel: T4PreprocessConfigurationViewModel
  private val lifetimeDefinition = Lifetime.Eternal.createNested()
  private val editorLifetime = SequentialLifetimes(lifetimeDefinition.lifetime)

  override fun disposeEditor() {
    lifetimeDefinition.terminate()
    super.disposeEditor()
  }

  override fun resetEditorFrom(configuration: T4PreprocessConfiguration) {
  }

  override fun createEditor(): JComponent {
    val lifetime = editorLifetime.next()
    viewModel = T4PreprocessConfigurationViewModel()
    return ControlViewBuilder(lifetime, project).build(viewModel)
  }

  override fun applyEditorTo(p0: T4PreprocessConfiguration) {
  }
}
