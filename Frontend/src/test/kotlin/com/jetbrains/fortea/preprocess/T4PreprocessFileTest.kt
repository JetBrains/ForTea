package com.jetbrains.fortea.preprocess

import com.jetbrains.rider.test.annotations.TestEnvironment
import com.jetbrains.rider.test.env.enums.SdkVersion
import org.testng.annotations.Test

@TestEnvironment(sdkVersion = SdkVersion.LATEST_STABLE)
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
  // https://youtrack.jetbrains.com/issue/RIDER-55555
  @Test fun `test file without linePragmas`() = doTest()
  @Test fun `test file with linePragmas false`() = doTest()
  @Test fun `test file with linePragmas true`() = doTest()
}