package com.jetbrains.fortea.run

import com.jetbrains.fortea.utils.T4TestHelper
import com.jetbrains.rider.projectView.solutionDirectoryPath
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.scriptingApi.combine
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test
import java.nio.file.Path
import kotlin.io.path.listDirectoryEntries
import kotlin.io.path.name

@Tag(TeamCityTags.Plugins.ForTea)
class T4RunFileInSubfolderTest : T4RunFileTestBase() {
  override fun createTestHelper() = object : T4TestHelper(project) {
    override val t4File: Path
      get() = project
        .solutionDirectoryPath
        .combine("Project")
        .combine("Subdirectory")
        .listDirectoryEntries().filter { it.name.endsWith(".tt") or it.name.endsWith(".t4") }
        .shouldNotBeNull()
        .single()
  }

  @Solution("testThatFileInSubdirectoryCanBeExecuted")
  @Test fun testThatFileInSubdirectoryCanBeExecuted() = doTest(dumpCsproj = false)
  @Solution("testHostSpecificFileWithIncludeAndReference")
  @Test fun testHostSpecificFileWithIncludeAndReference() = doTest(dumpCsproj = false)
}