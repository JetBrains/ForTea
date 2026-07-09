package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Test

@Solution("ProjectWith4AndIncluderInSubfolder")
class T4IncludedExecutableTemplateInSubfolderHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWith4AndIncluderInSubfolder"
  override val testFilePath get() = "Project/Folder/$fileName"
  @Test fun testDefaultClassesResolution() = doTestErrors()
}