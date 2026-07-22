package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea)
@Solution("ProjectWithT4AndIncluder")
class T4IncludedExecutableTemplateHighlightingTest : T4HighlightingTestBase() {
  // https://youtrack.jetbrains.com/issue/RIDER-36962
  @Test fun testReferenceResolutionInHostSpecificInclude() = doTestErrors()
}
