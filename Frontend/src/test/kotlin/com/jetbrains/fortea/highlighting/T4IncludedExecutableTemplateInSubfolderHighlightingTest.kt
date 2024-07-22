package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4IncludedExecutableTemplateInSubfolderHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWith4AndIncluderInSubfolder"
  override val testFilePath get() = "Project/Folder/$fileName"
  @Test fun testDefaultClassesResolution() = doTestErrors()
}