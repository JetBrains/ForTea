package com.jetbrains.fortea.run

import org.testng.annotations.Test

// Important note:
//   method names will at some point be parsed as program arguments,
//   so they cannot contain spaces!

class T4RunFileTest : T4RunFileTestBase() {
  @Test fun testThatFileCanBeExecuted() = doTest()
  @Test fun testThatHostSpecificTemplateCanBeExecuted() = doTest()
  @Test fun testThatHostCanSetResultExtension() {
    doTest(".cshtml")
    assertNoOutputWithExtension(".html")
  }

  @Test fun testThatTtincludeFileCanBeIncluded() = doTest()
  @Test fun testThatCSharpFileCanBeIncluded() = doTest()
  @Test fun testThatVsMacrosAreResolved() = doTest()
  @Test fun testThatMsBuildPropertiesAreResolved() = doTest()
  @Test fun testThatAssemblyCanBeReferenced() = doTest()
//  @Test fun testThatTransitiveDependenciesAreCollected() = doTest()
//  @Test fun testThatFileCanBeExecutedInDotNetCoreProject() = doTest()
  @Test fun testThatTemplateCanProduceBigXml() = doTest()
//  @Test fun testThatTemplateIsCaseInsensitive() = doTest()
//  @Test fun testThatFileExtensionCanBeUpdatedCorrectly() {
//    executeT4File()
//    t4File.writeText(t4File.readText().replace(".fs", ".cs"))
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
}
