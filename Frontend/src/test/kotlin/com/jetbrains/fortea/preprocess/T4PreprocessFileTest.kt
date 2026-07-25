package com.jetbrains.fortea.preprocess

import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.annotations.TestSettings
import com.jetbrains.rider.test.enums.BuildTool
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea.General)
@TestSettings(sdkVersion = SdkVersion.LATEST_STABLE, buildTool = BuildTool.SDK)
class T4PreprocessFileTest : T4PreprocessFileTestBase() {
  @Solution("test empty file in core project preprocessing")
  @Test fun `test empty file in core project preprocessing`() = doTest(dumpCsproj = true)
  @Solution("test empty file in classical project preprocessing")
  @Test fun `test empty file in classical project preprocessing`() = doTest(dumpCsproj = true)
  @Solution("test simple file preprocessing")
  @Test fun `test simple file preprocessing`() = doTest()
  @Solution("test parameter directive preprocessing")
  @Test fun `test parameter directive preprocessing`() = doTest()
  @Solution("test internal visibility preprocessing")
  @Test fun `test internal visibility preprocessing`() = doTest()
  @Solution("test hostspecific template preprocessing")
  @Test fun `test hostspecific template preprocessing`() = doTest()
  // https://youtrack.jetbrains.com/issue/RIDER-47615
  @Solution("test file with include preprocessing")
  @Test fun `test file with include preprocessing`() = doTest()
  // https://youtrack.jetbrains.com/issue/RIDER-69121
  @Solution("test namespace of preprocessed class 1")
  @Test fun `test namespace of preprocessed class 1`() = doTest()
  // https://youtrack.jetbrains.com/issue/RIDER-55555
  @Solution("test file without linePragmas")
  @Test fun `test file without linePragmas`() = doTest()
  @Solution("test file with linePragmas false")
  @Test fun `test file with linePragmas false`() = doTest()
  @Solution("test file with linePragmas true")
  @Test fun `test file with linePragmas true`() = doTest()
}