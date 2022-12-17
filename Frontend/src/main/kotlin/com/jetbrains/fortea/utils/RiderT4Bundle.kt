package com.jetbrains.fortea.utils

import com.intellij.DynamicBundle
import org.jetbrains.annotations.Nls
import org.jetbrains.annotations.NonNls
import org.jetbrains.annotations.PropertyKey
import java.util.function.Supplier

class RiderT4Bundle : DynamicBundle(BUNDLE) {
  companion object {
    @NonNls
    private const val BUNDLE = "messages.RiderT4Bundle"
    private val INSTANCE: RiderT4Bundle = RiderT4Bundle()

    @Nls
    fun message(
      @PropertyKey(resourceBundle = BUNDLE) key: String,
      vararg params: Any
    ): String {
      return INSTANCE.getMessage(key, *params)
    }

    fun messagePointer(
      @PropertyKey(resourceBundle = BUNDLE) key: String,
      vararg params: Any
    ): Supplier<String> {
      return INSTANCE.getLazyMessage(key, *params)
    }
  }
}