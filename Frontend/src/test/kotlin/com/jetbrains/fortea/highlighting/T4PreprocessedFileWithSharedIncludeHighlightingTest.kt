package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea)
@Solution("ProjectWithPreprocessedT4IncludedWithPartial")
class T4PreprocessedFileWithSharedIncludeHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithPreprocessedT4IncludedWithPartial"
  override val fileName = "Include.ttinclude"
  override val testFilePath: String
    get() = "Project/$fileName"

  @Mute("TODO: implement test")
  @Test
  fun `test swea in template included into multiple files`() = doTestErrors()
}