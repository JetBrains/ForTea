package com.jetbrains.fortea.utils

import com.intellij.openapi.project.Project
import com.jetbrains.rider.ideaInterop.vfs.VfsWriteOperationsHost
import com.jetbrains.rider.projectView.solutionDirectory
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.framework.combine
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.framework.flushQueues
import com.jetbrains.rider.test.scriptingApi.waitAllCommandsFinished
import com.jetbrains.rider.test.scriptingApi.waitForProjectModelReady
import com.jetbrains.rider.util.idea.application
import com.jetbrains.rider.util.idea.getComponent
import java.io.File

open class T4TestHelper(val project: Project) {
  open val t4File: File
    get() = project
      .solutionDirectory
      .combine("Project")
      .listFiles { _, name -> name.endsWith(".tt") or name.endsWith(".t4") }
      .shouldNotBeNull()
      .single()

  private val csprojFile
    get() = t4File
      .parentFile
      .listFiles { _, name -> name.endsWith(".csproj") }
      .shouldNotBeNull()
      .single()

  private val outputFileCandidates
    get() = t4File.parentFile.listFiles().shouldNotBeNull()

  fun saveSolution(project: Project) {
    application.saveAll()
    flushQueues()
    waitForProjectModelReady(project)
    waitAllCommandsFinished()
    project.getComponent<VfsWriteOperationsHost>().waitRefreshIsFinished()
  }

  fun dumpCsprojContents() = executeWithGold(csprojFile.path) {
    it.print(csprojFile.readText())
  }

  private fun findOutputFile(resultExtension: String?): File {
    return if (resultExtension == null) outputFileCandidates.single {
      it.nameWithoutExtension == t4File.nameWithoutExtension
        && it.name != t4File.name
        && !it.name.endsWith(".tmp")
        && !it.name.endsWith(".gold")
    } else outputFileCandidates.single {
      it.name == t4File.nameWithoutExtension + resultExtension
    }
  }

  fun dumpExecutionResult(resultExtension: String? = null) = executeWithGold(t4File.path) {
    it.print(findOutputFile(resultExtension).readText())
  }

  fun assertNoOutputWithExtension(extension: String) = assert(outputFileCandidates.none {
    it.nameWithoutExtension == t4File.nameWithoutExtension && it.name.endsWith(extension)
  })
}