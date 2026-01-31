package com.jetbrains.fortea.completion

import com.intellij.openapi.editor.impl.EditorImpl
import com.jetbrains.rider.test.base.CompletionTestBase
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.scriptingApi.assertCurrentLookupItemEquals
import com.jetbrains.rider.test.scriptingApi.assertLookupNotContains
import com.jetbrains.rider.test.scriptingApi.completeWithEnter
import com.jetbrains.rider.test.scriptingApi.dumpOpenedDocument
import com.jetbrains.rider.test.scriptingApi.ensureThereIsNoLookup
import com.jetbrains.rider.test.scriptingApi.pressEnter
import com.jetbrains.rider.test.scriptingApi.typeWithLatency
import com.jetbrains.rider.test.scriptingApi.undo
import com.jetbrains.rider.test.scriptingApi.withOpenedEditor
import org.testng.annotations.Ignore
import org.testng.annotations.Test

class T4CompletionInExecutableTest : CompletionTestBase() {
  override val testSolution = "ProjectWithT4"

  @Ignore("broken")
  @Test
  fun testClassName() = doTest {
    // correct name should be hidden. See https://github.com/JetBrains/ForTea/issues/28
    typeWithLatency("    GeneratedTextTra")
    ensureThereIsNoLookup()
    typeWithLatency("nsformation x = this;")
    pressEnter()

    // incorrect name should not be shown
    typeWithLatency("Templa")
    assertLookupNotContains("Template")
    typeWithLatency("te ")
    undo()
    typeWithLatency(" y = this;")
  }

  @Test
  fun testBaseClassName() = doTest {
    // The correct base class name
    typeWithLatency("TextTransformati")
    assertCurrentLookupItemEquals("TextTransformation")
    completeWithEnter()
    typeWithLatency(" x = this;")
    pressEnter()

    // The incorrect one
    typeWithLatency("TemplateBa")
    ensureThereIsNoLookup()
    typeWithLatency("se y = this;")
  }

  @Test
  fun testFunctionName() = doTest {
    // correct name should be hidden. See https://github.com/JetBrains/ForTea/issues/28
    typeWithLatency("TransformTe")
    ensureThereIsNoLookup()
    typeWithLatency("xt();")
    pressEnter()
  }

  private fun doTest(block: EditorImpl.() -> Unit) {
    withOpenedEditor("Template.tt") {
      typeWithLatency("<#")
      pressEnter()
      block()
      executeWithGold(testCaseGoldDirectory.resolve("Template.tt")) { printStream ->
        dumpOpenedDocument(printStream, project!!)
      }
    }
  }
}