package com.jetbrains.fortea.preprocess

import com.intellij.platform.backend.workspace.WorkspaceModel
import com.jetbrains.fortea.inTests.T4TestHost
import com.jetbrains.fortea.utils.T4TestHelper
import com.jetbrains.rdclient.protocol.protocolHost
import com.jetbrains.rdclient.util.idea.toVirtualFile
import com.jetbrains.fortea.model.T4FileLocation
import com.jetbrains.rider.projectView.workspace.getId
import com.jetbrains.rider.projectView.workspace.getProjectModelEntities
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.base.BaseTestWithSolution
import org.testng.annotations.AfterMethod
import org.testng.annotations.BeforeMethod

abstract class T4PreprocessFileTestBase : BaseTestWithSolution() {
  override fun getSolutionDirectoryName() = testMethod.name
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

  @BeforeMethod
  open fun setUp() {
    helper = T4TestHelper(project)
  }

  @AfterMethod
  fun tearDown() {
    helper = null
  }


  private fun preprocessT4File() {
    val virtualFile = helper!!.t4File.path.toVirtualFile(true).shouldNotBeNull()
    val projectModelEntity = WorkspaceModel.getInstance(project).getProjectModelEntities(virtualFile, project).single()
    val id = projectModelEntity.getId(project)!!
    val location = T4FileLocation(id)
    T4TestHost.getInstance(project.protocolHost).preprocessFile(location)
  }
}