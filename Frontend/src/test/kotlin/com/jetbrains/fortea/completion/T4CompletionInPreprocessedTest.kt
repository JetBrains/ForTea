package com.jetbrains.fortea.completion

import com.jetbrains.rider.test.base.CompletionTestBase
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.scriptingApi.assertCurrentLookupItemEquals
import com.jetbrains.rider.test.scriptingApi.completeWithEnter
import com.jetbrains.rider.test.scriptingApi.dumpOpenedDocument
import com.jetbrains.rider.test.scriptingApi.dumpOpenedEditor
import com.jetbrains.rider.test.scriptingApi.ensureThereIsNoLookup
import com.jetbrains.rider.test.scriptingApi.typeWithLatency
import com.jetbrains.rider.test.scriptingApi.withOpenedEditor
import org.testng.annotations.Test

class T4CompletionInPreprocessedTest : CompletionTestBase() {
  override val testSolution = "ProjectWithPreprocessedT4"

  @Test
  fun testClassName() = doTest("Templa", "Template")

  @Test
  fun testBaseClassName() = doTest("TemplateBa", "TemplateBase")

  @Test
  fun testMethodName() = doTest("Transform", "TransformText")

  @Test
  fun testIncorrectClassName() = doTest("GeneratedTextTransfo")

  @Test
  fun testIncorrectBaseClassName() = doTest("TextTransfo")

  private fun doTest(text: String, fullItem: String) {
    dumpOpenedEditor("Template.tt") {
      typeWithLatency("<# $text")
      assertCurrentLookupItemEquals(fullItem)
      completeWithEnter()
    }
  }

  private fun doTest(text: String) {
    withOpenedEditor("Template.tt") {
      typeWithLatency("<# $text")
      ensureThereIsNoLookup()
      executeWithGold(testCaseGoldDirectory.resolve("Template.tt")) {
        dumpOpenedDocument(it, project!!)
      }
    }
  }
}