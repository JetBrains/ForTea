package com.jetbrains.fortea.utils

import com.intellij.DynamicBundle
import org.jetbrains.annotations.Nls
import org.jetbrains.annotations.NonNls
import org.jetbrains.annotations.PropertyKey
import java.util.function.Supplier

object RiderT4Bundle {
  @NonNls
  private const val BUNDLE = "messages.RiderT4Bundle"
  private val instance = DynamicBundle(RiderT4Bundle::class.java, BUNDLE)

  @Nls
  fun message(
    @PropertyKey(resourceBundle = BUNDLE) key: String,
    vararg params: Any
  ): String {
    return instance.getMessage(key, *params)
  }

  fun messagePointer(
    @PropertyKey(resourceBundle = BUNDLE) key: String,
    vararg params: Any
  ): Supplier<String> {
    return instance.getLazyMessage(key, *params)
  }
}