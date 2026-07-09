package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.Tags
import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(Tags.Episode.ForTea)
@Solution("ProjectWith4AndIncluderInSubfolder")
class T4IncludedExecutableTemplateInSubfolderHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWith4AndIncluderInSubfolder"
  override val testFilePath get() = "Project/Folder/$fileName"
  @Test fun testDefaultClassesResolution() = doTestErrors()
}