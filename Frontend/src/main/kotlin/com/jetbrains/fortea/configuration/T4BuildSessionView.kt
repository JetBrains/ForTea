package com.jetbrains.fortea.configuration

import com.jetbrains.rider.model.T4BuildResult
import com.jetbrains.rider.model.T4PreprocessingResult

interface T4BuildSessionView {
  fun openWindow(message: String)
  fun showT4BuildResult(result: T4BuildResult, file: String)
  fun showT4PreprocessingResult(result: T4PreprocessingResult, file: String)
}
