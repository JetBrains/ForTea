package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.HighlightSeverity
import com.jetbrains.rider.test.annotations.Mute
import org.testng.annotations.Test

class T4ExecutableFileInSubfolderHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "testHostSpecificFileWithIncludeAndReference"
  override val testFilePath get() = "Project/Subdirectory/$fileName"

  @Test fun testHostAssemblyInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testNoHostAssemblyInclude() = doTest(HighlightSeverity.ERROR)
  @Mute("RIDER-98398")
  @Test fun testHostNoAssemblyInclude() = doTest(HighlightSeverity.ERROR)
  @Mute("RIDER-98398")
  @Test fun testNoHostNoAssemblyInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testHostAssemblyNoInclude() = doTest(HighlightSeverity.ERROR)
  @Test fun testNoHostAssemblyNoInclude() = doTest(HighlightSeverity.ERROR)
  @Mute("RIDER-98398")
  @Test fun testHostNoAssemblyNoInclude() = doTest(HighlightSeverity.ERROR)
  @Mute("RIDER-98398")
  @Test fun testNoHostNoAssemblyNoInclude() = doTest(HighlightSeverity.ERROR)
}