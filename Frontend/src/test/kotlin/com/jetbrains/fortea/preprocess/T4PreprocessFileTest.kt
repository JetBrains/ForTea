package com.jetbrains.fortea.preprocess

import com.jetbrains.rider.test.annotations.TestEnvironment
import com.jetbrains.rider.test.enums.ToolsetVersion
import com.jetbrains.rider.test.enums.CoreVersion
import org.testng.annotations.Test

@TestEnvironment(toolset = ToolsetVersion.TOOLSET_16_CORE, coreVersion = CoreVersion.DEFAULT)
class T4PreprocessFileTest : T4PreprocessFileTestBase() {
  @Test fun `test empty file in core project preprocessing`() = doTest(dumpCsproj = true)
  @Test fun `test empty file in classical project preprocessing`() = doTest(dumpCsproj = true)
  @Test fun `test simple file preprocessing`() = doTest()
  @Test fun `test parameter directive preprocessing`() = doTest()
  @Test fun `test internal visibility preprocessing`() = doTest()
  @Test fun `test hostspecific template preprocessing`() = doTest()
  // https://youtrack.jetbrains.com/issue/RIDER-47615
  @Test fun `test file with include preprocessing`() = doTest()
  // https://youtrack.jetbrains.com/issue/RIDER-69121
  @Test fun `test namespace of preprocessed class 1`() = doTest()
}