package com.jetbrains.fortea.configuration.run.task

import com.intellij.execution.BeforeRunTask

class T4CompileBeforeRunTask : BeforeRunTask<T4CompileBeforeRunTask>(T4CompileBeforeRunTaskProvider.providerId)

class T4BuildProjectsBeforeRunTask :
  BeforeRunTask<T4BuildProjectsBeforeRunTask>(T4BuildProjectsBeforeRunTaskProvider.providerId)
