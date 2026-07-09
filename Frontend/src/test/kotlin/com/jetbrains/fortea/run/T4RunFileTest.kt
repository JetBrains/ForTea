package com.jetbrains.fortea.run

import com.jetbrains.fortea.Tags
import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.annotations.TestSettings
import com.jetbrains.rider.test.enums.BuildTool
import com.jetbrains.rider.test.enums.Mono
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

// Note: due to Windows path length restriction
// test method name cannot be longer than 60 symbols
@Tag(Tags.Episode.ForTea)
@TestSettings(sdkVersion = SdkVersion.LATEST_STABLE, buildTool = BuildTool.SDK, mono = Mono.UNIX_ONLY)
class T4RunFileTest : T4RunFileTestBase() {
  @Solution("testThatFileCanBeExecuted")
  @Test fun testThatFileCanBeExecuted() = doTest()
  @Solution("testThatHostSpecificTemplateCanBeExecuted")
  @Test fun testThatHostSpecificTemplateCanBeExecuted() = doTest()
  @Solution("testThatHostCanSetResultExtension")
  @Test fun testThatHostCanSetResultExtension() {
    doTest(".cshtml")
    helper.assertNoOutputWithExtension(".html")
  }

  @Solution("testThatTtincludeFileCanBeIncluded")
  @Test fun testThatTtincludeFileCanBeIncluded() = doTest()
  @Solution("testThatCSharpFileCanBeIncluded")
  @Test fun testThatCSharpFileCanBeIncluded() = doTest()
  @Solution("testThatVsMacrosAreResolved")
  @Test fun testThatVsMacrosAreResolved() = doTest()
  @Solution("testThatMsBuildPropertiesAreResolved")
  @Test fun testThatMsBuildPropertiesAreResolved() = doTest()
  @Solution("testThatAssemblyCanBeReferenced")
  @Test fun testThatAssemblyCanBeReferenced() = doTest()
  @Solution("testTransitiveReferencesInRuntime")
  @Test fun testTransitiveReferencesInRuntime() = doTest()
  @Solution("testTransitiveReferencesInCompilation")
  @Test fun testTransitiveReferencesInCompilation() = testExecutionFailure(".cs")
  @Solution("testThatFileCanBeExecutedInDotNetCoreProject")
  @Test fun testThatFileCanBeExecutedInDotNetCoreProject() = doTest()
  @Solution("testThatTemplateCanProduceBigXml")
  @Test fun testThatTemplateCanProduceBigXml() = doTest()
  @Solution("testThatTemplateIsCaseInsensitive")
  @Test fun testThatTemplateIsCaseInsensitive() = doTest()
//  @Test fun testThatFileExtensionCanBeUpdatedCorrectly() {
//    executeT4File()
//    t4File.writeText(t4File.readText().replace(".fs", ".cs"))
//    todo: saveSolution()? or see @korifey dialog for details on how to force update from disk
//    executeT4File()
//    saveSolution()
//    dumpExecutionResult(".cs")
//    dumpCsproj()
//    assertNoOutputWithExtension(".fs")
//  }
  @Solution("testThatVsDefaultTemplateCanBeExecuted")
  @Test fun testThatVsDefaultTemplateCanBeExecuted() = doTest()
  @Solution("testThatDefaultExtensionIsCs")
  @Test fun testThatDefaultExtensionIsCs() = doTest(".cs")
  @Solution("testThatFileWithT4ExtensionCanBeExecuted")
  @Test fun testThatFileWithT4ExtensionCanBeExecuted() = doTest()
  @Solution("testThatExtensionCanContainDot")
  @Test fun testThatExtensionCanContainDot() = doTest(".txt")
  @Solution("testThatExtensionCanBeWithoutDot")
  @Test fun testThatExtensionCanBeWithoutDot() = doTest(".txt")
  @Solution("testTemplateWithLineBreakMess")
  @Test fun testTemplateWithLineBreakMess() = doTest()
  @Solution("testThatFeatureBlocksCanContainManyNewLines")
  @Test fun testThatFeatureBlocksCanContainManyNewLines() = doTest()
  @Solution("testHowTextInFeatureIsHandled")
  @Test fun testHowTextInFeatureIsHandled() = doTest()
//  @Test fun testThatOutputOfUnbuiltProjectCanBeReferenced() = doTest()
  @Solution("testHostInHostSpecificTemplate")
  @Test fun testHostInHostSpecificTemplate() = doTest()
  @Solution("testHostInNonHostSpecificTemplate")
  @Test fun testHostInNonHostSpecificTemplate() = testExecutionFailure(".txt", true)
  @Solution("testInProjectTransitiveIncludeResolution")
  @Test fun testInProjectTransitiveIncludeResolution() = doTest()
  @Solution("testOutOfProjectTransitiveIncludeResolution")
  @Test fun testOutOfProjectTransitiveIncludeResolution() = doTest()
  @Solution("testInProjectNonTrivialIncludeResolution")
  @Test fun testInProjectNonTrivialIncludeResolution() = doTest()
  @Solution("test execution with spaces in path")
  @Test fun `test execution with spaces in path`() = doTest(dumpCsproj = false)
  @Solution("test that Program_tt can be executed")
  @Test fun `test that Program_tt can be executed`() = doTest(dumpCsproj = false)
  @Solution("test that Program_tt can be executed 2")
  @Test fun `test that Program_tt can be executed 2`() = doTest(dumpCsproj = false)
  @Solution("test access to ValueTuple")
  @Test fun `test access to ValueTuple`() = doTest(dumpCsproj = false)

  @Mute("Broken on the buildserver")
  @Solution("test access to ValueTuple in old framework")
  @Test
  fun `test access to ValueTuple in old framework`() = testExecutionFailure(".txt")

  @Solution("test that TextTransformation is like in VS")
  @Test fun `test that TextTransformation is like in VS`() = doTest(dumpCsproj = false)
  @Solution("test that host resolves an empty string")
  @Test fun `test that host resolves an empty string`() = doTest(dumpCsproj = false)
  @Solution("test how host resolves null")
  @Test fun `test how host resolves null`() = testExecutionFailure(".txt")
  @Solution("test file with a macro twice")
  @Test fun `test file with a macro twice`() = doTest(dumpCsproj = false)
  @Solution("test default references")
  @Test fun `test default references`() = doTest(dumpCsproj = false)
  @Mute
  @Solution("test host specific template references")
  @Test fun `test host specific template references`() = doTest(dumpCsproj = false)
  // https://youtrack.jetbrains.com/issue/RIDER-69121
  @Solution("test namespace of generate class 1")
  @Test fun `test namespace of generate class 1`() = doTest(dumpCsproj = false)
  @Solution("test namespace of generate class 2")
  @Test fun `test namespace of generate class 2`() = testExecutionFailure(".cs")
}
