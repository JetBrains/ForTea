package com.jetbrains.fortea.highlighting

import com.jetbrains.rider.test.annotations.Mute
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea)
@Solution("ProjectWithT4IncludedInSubfolder")
class T4IncludedPreprocessedTemplateInSubfolderHighlightingTest : T4HighlightingTestBase() {
  override val testFilePath: String
    get() = "$activeSolution/Folder/$fileName"

  @Mute("FIXME")
  @Test
  fun testClassName() = doTestErrors()
}