package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.Tags
import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(Tags.Episode.ForTea)
@Solution("ProjectWithPreprocessedT4IncludedWithPartial")
class T4IncludedPreprocessedTemplatePartialTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithPreprocessedT4IncludedWithPartial"
  override val fileName = "Include.ttinclude"
  override val testFilePath: String
    get() = "Project/$fileName"

  @Mute("TODO: figure out how to wait for PSI module rebuild")
  @Test
  fun `test partial class resolution in preprocessed include`() = doTestErrors()

  @Mute("TODO: figure out how to wait for PSI module rebuild")
  @Test
  fun `test that TransformText is resolved in preprocessed include`() = doTestErrors()
}