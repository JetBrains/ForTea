package com.jetbrains.fortea.run

import com.jetbrains.fortea.utils.T4TestHelper
import com.jetbrains.rider.projectView.solutionDirectoryPath
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.framework.combine
import org.testng.annotations.Test
import java.nio.file.Path
import kotlin.io.path.listDirectoryEntries
import kotlin.io.path.name

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

  @Test fun testThatFileInSubdirectoryCanBeExecuted() = doTest(dumpCsproj = false)
  @Test fun testHostSpecificFileWithIncludeAndReference() = doTest(dumpCsproj = false)
}