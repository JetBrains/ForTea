package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Test

@Solution("ProjectWithT4AndIncluder")
class T4IncludedExecutableTemplateHighlightingTest : T4HighlightingTestBase() {
  override val testSolution = "ProjectWithT4AndIncluder"

  // https://youtrack.jetbrains.com/issue/RIDER-36962
  @Test fun testReferenceResolutionInHostSpecificInclude() = doTestErrors()
}
