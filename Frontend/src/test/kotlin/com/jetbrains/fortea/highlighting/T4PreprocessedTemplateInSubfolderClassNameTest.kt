package com.jetbrains.fortea.highlighting

import com.jetbrains.fortea.Tags
import com.jetbrains.rider.test.annotations.Solution
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(Tags.Episode.ForTea)
@Solution("ProjectWithComplexPreprocessedT4")
class T4PreprocessedTemplateInSubfolderClassNameTest  : T4HighlightingTestBase() {
  override val testSolution: String = "ProjectWithComplexPreprocessedT4"
  override val testFilePath: String
    get() = "$testSolution/Folder/$fileName"

  @Test fun testClassName() = doTestErrors()
}