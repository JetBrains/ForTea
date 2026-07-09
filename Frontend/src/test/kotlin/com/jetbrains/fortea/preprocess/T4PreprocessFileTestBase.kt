package com.jetbrains.fortea.preprocess

import com.intellij.platform.backend.workspace.WorkspaceModel
import com.jetbrains.fortea.inTests.preprocessFile
import com.jetbrains.fortea.model.T4FileLocation
import com.jetbrains.fortea.utils.T4TestHelper
import com.jetbrains.rdclient.util.idea.toVirtualFile
import com.jetbrains.rider.projectView.workspace.getId
import com.jetbrains.rider.projectView.workspace.getProjectModelEntities
import com.jetbrains.rider.protocol.protocol
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.junit5.base.PerTestSolutionTestBase
import org.junit.jupiter.api.AfterEach
import org.junit.jupiter.api.BeforeEach
import kotlin.io.path.pathString

abstract class T4PreprocessFileTestBase : PerTestSolutionTestBase() {
  private var helper: T4TestHelper? = null

  protected fun doTest(dumpCsproj: Boolean = false) {
    preprocessT4File()
    helper!!.saveSolution(project)
    helper!!.dumpExecutionResult(printer = {
      it.replace("""#line (?<lineNumber>\d+) ".*[/\\](?<fileName>[^\\/\"]+)"""".toRegex()) { match ->
        "#line ${match.groups["lineNumber"]!!.value} \".../${match.groups["fileName"]!!.value}\""
      }
    })
    if (dumpCsproj) helper!!.dumpCsprojContents()
  }

  @BeforeEach
  open fun setUp() {
    helper = T4TestHelper(project)
  }

  @AfterEach
  fun tearDown() {
    helper = null
  }


  private fun preprocessT4File() {
    val virtualFile = helper!!.t4File.pathString.toVirtualFile(true).shouldNotBeNull()
    val projectModelEntity = WorkspaceModel.getInstance(project).getProjectModelEntities(virtualFile, project).single()
    val id = projectModelEntity.getId(project)!!
    val location = T4FileLocation(id)
    project.protocol.preprocessFile(location)
  }
}