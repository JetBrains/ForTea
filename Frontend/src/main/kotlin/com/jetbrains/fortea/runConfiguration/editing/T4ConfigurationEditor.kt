package com.jetbrains.fortea.runConfiguration.editing

import com.intellij.execution.ExecutionBundle
import com.intellij.openapi.fileChooser.FileChooserDescriptor
import com.intellij.openapi.fileChooser.FileChooserDescriptorFactory
import com.intellij.openapi.options.SettingsEditor
import com.intellij.openapi.project.Project
import com.intellij.openapi.rd.createNestedDisposable
import com.intellij.openapi.util.Comparing
import com.intellij.openapi.util.SystemInfo
import com.intellij.openapi.util.io.FileUtil
import com.jetbrains.fortea.runConfiguration.T4Configuration
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.lifetime.SequentialLifetimes
import com.jetbrains.rdclient.protocol.IPermittedModalities
import com.jetbrains.rider.run.configurations.controls.*
import javax.swing.JComponent

class T4ConfigurationEditor(private val project: Project) : SettingsEditor<T4Configuration>() {

  private lateinit var viewModel: T4ConfigurationViewModel
  private val lifetimeDefinition = Lifetime.Eternal.createNested()
  private val editorLifetime = SequentialLifetimes(lifetimeDefinition.lifetime)

  override fun disposeEditor() {
    lifetimeDefinition.terminate()
    super.disposeEditor()
  }

  override fun resetEditorFrom(configuration: T4Configuration) {
    configuration.parameters.apply {
      viewModel.reset(
        exePath,
        programParameters,
        workingDirectory,
        envs,
        isPassParentEnvs,
        useMonoRuntime,
        runtimeArguments,
        useExternalConsole
      )
    }
  }

  override fun applyEditorTo(configuration: T4Configuration) {
    configuration.parameters.exePath = FileUtil.toSystemIndependentName(viewModel.exePathSelector.path.value)
    configuration.parameters.programParameters =
      FileUtil.toSystemIndependentName(viewModel.programParametersEditor.parametersString.value)
    configuration.parameters.workingDirectory =
      FileUtil.toSystemIndependentName(viewModel.workingDirectorySelector.path.value)
    configuration.parameters.envs = viewModel.environmentVariablesEditor.envs.value
    configuration.parameters.isPassParentEnvs = viewModel.environmentVariablesEditor.isPassParentEnvs.value
    configuration.parameters.runtimeArguments = viewModel.runtimeArgumentsEditor.parametersString.value
    configuration.parameters.useMonoRuntime = viewModel.useMonoRuntimeEditor.isSelected.value
    configuration.parameters.useExternalConsole = viewModel.useExternalConsoleEditor.isSelected.value
  }

  override fun createEditor(): JComponent {
    val lifetime = editorLifetime.next()
    viewModel = T4ConfigurationViewModel(
      lifetime,
      PathSelector("Exe path:", FileChooserDescriptor(
        true, false, false,
        false, false, false
      ).withFileFilter { file ->
        Comparing.equal(file.extension, "exe", SystemInfo.isFileSystemCaseSensitive)
          || Comparing.equal(file.extension, "dll", SystemInfo.isFileSystemCaseSensitive)
      }, lifetime
      ),
      ProgramParametersEditor(ExecutionBundle.message("run.configuration.program.parameters"), lifetime),
      PathSelector("Working directory:", FileChooserDescriptorFactory.createSingleFolderDescriptor(), lifetime),
      EnvironmentVariablesEditor("Environment variables:"),
      FlagEditor("Use Mono runtime"),
      ProgramParametersEditor("Runtime arguments:", lifetime),
      true,
      FlagEditor("Use external console")
    )

    val editor = ControlViewBuilder(lifetime, project).build(viewModel)
    IPermittedModalities.getInstance().allowPumpProtocolForComponent(
      editor,
      lifetime.createNestedDisposable("T4ConfigurationEditor")
    )
    return editor
  }
}
