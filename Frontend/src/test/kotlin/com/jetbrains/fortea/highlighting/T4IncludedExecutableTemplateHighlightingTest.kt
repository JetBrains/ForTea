package com.jetbrains.fortea.highlighting

import org.testng.annotations.Test

class T4IncludedExecutableTemplateHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithT4AndIncluder"

  // https://youtrack.jetbrains.com/issue/RIDER-36962
  @Test fun testReferenceResolutionInHostSpecificInclude() = doTestErrors()
}
