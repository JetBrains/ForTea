package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4TemplateInMultiTargetProjectHighlightingTest : T4HighlightingTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithMultipleTargetFrameworks"
  override val testFilePath get() = "Project/$fileName"

  @Test fun testSimpleFile() = doTestErrors()
  @Test fun testFileWithMsBuildProperty() = doTestErrors()
  @Test fun testFunctionFromIncluder() = doTestErrors()
}