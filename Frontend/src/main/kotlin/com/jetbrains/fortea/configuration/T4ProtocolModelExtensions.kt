package com.jetbrains.fortea.configuration

import com.jetbrains.rider.model.BuildResultKind
import com.jetbrains.rider.model.T4BuildResultKind

val T4BuildResultKind.isSuccess
  get() = when (this) {
    T4BuildResultKind.HasErrors -> false
    T4BuildResultKind.HasWarnings -> true
    T4BuildResultKind.Successful -> true
  }

val T4BuildResultKind.toBuildResultKind
  get() = when (this) {
    T4BuildResultKind.HasErrors -> BuildResultKind.HasErrors
    T4BuildResultKind.HasWarnings -> BuildResultKind.HasWarnings
    T4BuildResultKind.Successful -> BuildResultKind.Successful
  }
