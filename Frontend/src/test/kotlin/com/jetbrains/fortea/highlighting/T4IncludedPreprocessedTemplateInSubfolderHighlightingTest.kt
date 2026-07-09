package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Test

@Solution("ProjectWithT4IncludedInSubfolder")
class T4IncludedPreprocessedTemplateInSubfolderHighlightingTest : T4HighlightingTestBase() {
  override val testSolution: String = "ProjectWithT4IncludedInSubfolder"
  override val testFilePath: String
    get() = "$testSolution/Folder/$fileName"

  @Mute("FIXME")
  @Test
  fun testClassName() = doTestErrors()
}