package com.jetbrains.fortea.run

import com.jetbrains.rider.projectView.solutionDirectory
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.framework.combine
import org.testng.annotations.Test
import java.io.File

class T4RunFileInSubfolderTest : T4RunFileTestBase() {
  override val t4File: File
    get() = project
      .solutionDirectory
      .combine("Project")
      .combine("Subdirectory")
      .listFiles { _, name -> name.endsWith(".tt") or name.endsWith(".t4") }
      .shouldNotBeNull()
      .single()

  @Test fun testThatFileInSubdirectoryCanBeExecuted() = doTest(dumpCsproj = false)
  @Test fun testHostSpecificFileWithIncludeAndReference() = doTest(dumpCsproj = false)
}