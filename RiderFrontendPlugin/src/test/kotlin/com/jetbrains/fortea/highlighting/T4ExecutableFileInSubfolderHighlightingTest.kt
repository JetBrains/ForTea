package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.HighlightSeverity
import org.testng.annotations.Test

class T4ExecutableFileInSubfolderHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "testHostSpecificFileWithIncludeAndReference"
  override val testFilePath get() = "Project/Subdirectory/$fileName"

  @Test fun testHostAssemblyInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testNoHostAssemblyInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testHostNoAssemblyInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testNoHostNoAssemblyInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testHostAssemblyNoInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testNoHostAssemblyNoInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testHostNoAssemblyNoInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testNoHostNoAssemblyNoInclude() = doTest(HighlightSeverity.ERROR)
}