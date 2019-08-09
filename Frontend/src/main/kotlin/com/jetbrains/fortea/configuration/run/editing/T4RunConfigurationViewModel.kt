package com.jetbrains.fortea.configuration.run.editing

import com.intellij.openapi.util.SystemInfo
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rider.run.configurations.controls.*
import com.jetbrains.rider.run.configurations.exe.ExeConfigurationViewModel

class T4RunConfigurationViewModel(
  lifetime: Lifetime,
  exePathSelector: PathSelector,
  programParametersEditor: ProgramParametersEditor,
  workingDirectorySelector: PathSelector,
  environmentVariablesEditor: EnvironmentVariablesEditor,
  val useMonoRuntimeEditor: FlagEditor,
  val runtimeArgumentsEditor: ProgramParametersEditor,
  trackExePathInWorkingDirectoryIfItPossible: Boolean,
  useExternalConsoleEditor: FlagEditor
) : ExeConfigurationViewModel(
  lifetime,
  exePathSelector,
  programParametersEditor,
  workingDirectorySelector,
  environmentVariablesEditor,
  useExternalConsoleEditor,
  trackExePathInWorkingDirectoryIfItPossible
) {

  override val controls: List<ControlBase>
    get() = listOf(
      exePathSelector,
      programParametersEditor,
      workingDirectorySelector,
      environmentVariablesEditor,
      runtimeArgumentsEditor,
      useMonoRuntimeEditor,
      useExternalConsoleEditor
    )

  init {
    useMonoRuntimeEditor.isVisible.set(SystemInfo.isWindows)
  }

  fun reset(
    exePath: String,
    programParameters: String,
    workingDirectory: String,
    envs: Map<String, String>,
    isPassParentEnvs: Boolean,
    useMonoRuntime: Boolean,
    runtimeOptions: String,
    useExternalConsole: Boolean
  ) {
    exePathSelector.path.set(exePath)
    programParametersEditor.parametersString.set(programParameters)
    workingDirectorySelector.path.set(workingDirectory)
    environmentVariablesEditor.envs.set(envs)
    environmentVariablesEditor.isPassParentEnvs.set(isPassParentEnvs)
    useMonoRuntimeEditor.isSelected.set(useMonoRuntime)
    runtimeArgumentsEditor.parametersString.set(runtimeOptions)
    useExternalConsoleEditor.isSelected.set(useExternalConsole)
  }
}
