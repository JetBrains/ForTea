package com.jetbrains.fortea.run

import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.TestEnvironment
import com.jetbrains.rider.test.enums.sdk.SdkVersion
import org.testng.annotations.Ignore
import org.testng.annotations.Test

// Note: due to Windows path length restriction
// test method name cannot be longer than 60 symbols
@TestEnvironment(sdkVersion = SdkVersion.LATEST_STABLE)
class T4RunFileTest : T4RunFileTestBase() {
  @Test fun testThatFileCanBeExecuted() = doTest()
  @Test fun testThatHostSpecificTemplateCanBeExecuted() = doTest()
  @Test fun testThatHostCanSetResultExtension() {
    doTest(".cshtml")
    helper.assertNoOutputWithExtension(".html")
  }

  @Test fun testThatTtincludeFileCanBeIncluded() = doTest()
  @Test fun testThatCSharpFileCanBeIncluded() = doTest()
  @Test fun testThatVsMacrosAreResolved() = doTest()
  @Test fun testThatMsBuildPropertiesAreResolved() = doTest()
  @Test fun testThatAssemblyCanBeReferenced() = doTest()
  @Test fun testTransitiveReferencesInRuntime() = doTest()
  @Test fun testTransitiveReferencesInCompilation() = testExecutionFailure(".cs")
  @Test fun testThatFileCanBeExecutedInDotNetCoreProject() = doTest()
  @Test fun testThatTemplateCanProduceBigXml() = doTest()
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
  @Test fun testThatVsDefaultTemplateCanBeExecuted() = doTest()
  @Test fun testThatDefaultExtensionIsCs() = doTest(".cs")
  @Test fun testThatFileWithT4ExtensionCanBeExecuted() = doTest()
  @Test fun testThatExtensionCanContainDot() = doTest(".txt")
  @Test fun testThatExtensionCanBeWithoutDot() = doTest(".txt")
  @Test fun testTemplateWithLineBreakMess() = doTest()
  @Test fun testThatFeatureBlocksCanContainManyNewLines() = doTest()
  @Test fun testHowTextInFeatureIsHandled() = doTest()
//  @Test fun testThatOutputOfUnbuiltProjectCanBeReferenced() = doTest()
  @Test fun testHostInHostSpecificTemplate() = doTest()
  @Test fun testHostInNonHostSpecificTemplate() = testExecutionFailure(".txt", true)
  @Test fun testInProjectTransitiveIncludeResolution() = doTest()
  @Test fun testOutOfProjectTransitiveIncludeResolution() = doTest()
  @Test fun testInProjectNonTrivialIncludeResolution() = doTest()
  @Test fun `test execution with spaces in path`() = doTest(dumpCsproj = false)
  @Test fun `test that Program_tt can be executed`() = doTest(dumpCsproj = false)
  @Test fun `test that Program_tt can be executed 2`() = doTest(dumpCsproj = false)
  @Test fun `test access to ValueTuple`() = doTest(dumpCsproj = false)

  @Ignore("Broken on the buildserver")
  @Test
  fun `test access to ValueTuple in old framework`() = testExecutionFailure(".txt")

  @Test fun `test that TextTransformation is like in VS`() = doTest(dumpCsproj = false)
  @Test fun `test that host resolves an empty string`() = doTest(dumpCsproj = false)
  @Test fun `test how host resolves null`() = testExecutionFailure(".txt")
  @Test fun `test file with a macro twice`() = doTest(dumpCsproj = false)
  @Test fun `test default references`() = doTest(dumpCsproj = false)
  @Ignore
  @Test fun `test host specific template references`() = doTest(dumpCsproj = false)
  @Mute("RIDER-98455")
  @Test fun `test that host specific template can access EnvDTE`() = doTest(dumpCsproj = false)
  @Mute("RIDER-98455")
  @Test fun `test basic DTE functions`() = doTest(dumpCsproj = false)
  @Mute("RIDER-98455")
  @Test fun `test solution functions`() = doTest(dumpCsproj = false)
  @Mute("RIDER-98455")
  @Test fun `test project functions`() = doTest(dumpCsproj = false)
  @Mute("RIDER-98455")
  @Test fun `test AST functions`() = doTest(dumpCsproj = false)
  // https://youtrack.jetbrains.com/issue/RIDER-69121
  @Test fun `test namespace of generate class 1`() = doTest(dumpCsproj = false)
  @Test fun `test namespace of generate class 2`() = testExecutionFailure(".cs")
}
