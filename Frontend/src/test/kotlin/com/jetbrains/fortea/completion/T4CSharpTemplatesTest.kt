package com.jetbrains.fortea.completion

import com.jetbrains.fortea.inTests.waitForIndirectInvalidation
import com.jetbrains.rider.protocol.protocol
import com.jetbrains.rider.test.base.CompletionTestBase
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.scriptingApi.*
import org.testng.annotations.Test

class T4CSharpTemplatesTest : CompletionTestBase() {
  override fun getSolutionDirectoryName() = "ProjectWithT4"

  @Test
  fun testForeach() {
    executeWithGold(testCaseGoldDirectory.resolve("Template.tt")) { printStream ->
      withOpenedEditor("Template.tt") {
        project!!.protocol.waitForIndirectInvalidation()
        typeWithLatency("<#")
        pressEnter()
        typeWithLatency("    var data = new[] {1, 2, 3};")
        pressEnter()
        typeWithLatency("data.foreach")
        callBasicCompletion()
        completeWithEnter()
        completeWithEnter()
        dumpOpenedDocument(printStream, project!!)
      }
    }
  }
}