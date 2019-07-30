package com.jetbrains.fortea.configuration.run.task

import com.intellij.execution.BeforeRunTask

class CompileT4BeforeRunTask : BeforeRunTask<CompileT4BeforeRunTask>(CompileT4BeforeRunTaskProvider.providerId)
