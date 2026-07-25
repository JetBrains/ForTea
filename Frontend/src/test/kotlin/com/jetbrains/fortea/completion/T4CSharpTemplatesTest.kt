package com.jetbrains.fortea.completion

import com.jetbrains.fortea.inTests.waitForIndirectInvalidation
import com.jetbrains.rider.protocol.protocol
import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.junit5.base.CompletionTestBase
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.scriptingApi.callBasicCompletion
import com.jetbrains.rider.test.scriptingApi.completeWithEnter
import com.jetbrains.rider.test.scriptingApi.dumpOpenedDocument
import com.jetbrains.rider.test.scriptingApi.pressEnter
import com.jetbrains.rider.test.scriptingApi.typeWithLatency
import com.jetbrains.rider.test.scriptingApi.withOpenedEditor
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea.General)
@Solution("ProjectWithT4")
class T4CSharpTemplatesTest : CompletionTestBase() {
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