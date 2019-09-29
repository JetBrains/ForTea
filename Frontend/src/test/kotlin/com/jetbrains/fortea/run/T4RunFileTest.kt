package com.jetbrains.fortea.run

import org.testng.annotations.Test

// Important note:
//   method names will at some point be parsed as program arguments,
//   so they cannot contain spaces!

class T4RunFileTest : T4RunFileTestBase() {
  @Test fun testThatFileCanBeExecuted() = doTest()
  @Test fun testThatHostSpecificTemplateCanBeExecuted() = doTest()
  @Test fun testThatHostCanSetResultExtension() = doTest(".cshtml")
  @Test fun testThatTtincludeFileCanBeIncluded() = doTest()
  @Test fun testThatCSharpFileCanBeIncluded() = doTest()
  @Test fun testThatVsMacrosAreResolved() = doTest()
  @Test fun testThatMsBuildPropertiesAreResolved() = doTest()
  @Test fun testThatAssemblyCanBeReferenced() = doTest()
  // @Test fun testThatTransitiveDependenciesAreCollected() = doTest()
}
