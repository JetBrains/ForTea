package com.jetbrains.fortea.utils

import com.intellij.openapi.project.Project
import com.intellij.util.application
import com.jetbrains.rider.ideaInterop.vfs.VfsWriteOperationsHost
import com.jetbrains.rider.projectView.solutionDirectoryPath
import com.jetbrains.rider.test.asserts.shouldNotBeNull
import com.jetbrains.rider.test.framework.combine
import com.jetbrains.rider.test.framework.compareXml
import com.jetbrains.rider.test.framework.executeWithGold
import com.jetbrains.rider.test.framework.flushQueues
import com.jetbrains.rider.test.scriptingApi.waitAllCommandsFinished
import com.jetbrains.rider.test.scriptingApi.waitForProjectModelReady
import com.jetbrains.rider.test.scriptingApi.waitRefreshIsFinished
import java.nio.file.Files
import java.nio.file.Path
import kotlin.io.path.listDirectoryEntries
import kotlin.io.path.name
import kotlin.io.path.nameWithoutExtension
import kotlin.io.path.readText

open class T4TestHelper(val project: Project) {
  open val t4File: Path
    get() = project
      .solutionDirectoryPath
      .combine("Project")
      .listDirectoryEntries().filter { it.name.endsWith(".tt") or it.name.endsWith(".t4") }
      .shouldNotBeNull()
      .single()

  private val csprojFile
    get() = t4File
      .parent
      .listDirectoryEntries().filter { it.name.endsWith(".csproj") }
      .shouldNotBeNull()
      .single()

  private val outputFileCandidates
    get() = t4File.parent.listDirectoryEntries().shouldNotBeNull()

  fun saveSolution(project: Project) {
    application.saveAll()
    flushQueues()
    waitForProjectModelReady(project)
    waitAllCommandsFinished()
    VfsWriteOperationsHost.getInstance(project).waitRefreshIsFinished()
  }

  fun dumpCsprojContents() = executeWithGold(csprojFile,
    action = { it.print(csprojFile.readText()) },
    comparator = { goldFile, tempFile -> compareXml("", goldFile, tempFile, "") }
  )

  private fun findOutputFile(resultExtension: String?): Path {
    return if (resultExtension == null) outputFileCandidates.single {
      it.nameWithoutExtension == t4File.nameWithoutExtension
        && it.name != t4File.name
        && !it.name.endsWith(".tmp")
        && !it.name.endsWith(".gold")
    } else outputFileCandidates.single {
      it.name == t4File.nameWithoutExtension + resultExtension
    }
  }

  fun dumpExecutionResult(resultExtension: String? = null, printer: ((String) -> String)? = null) = executeWithGold(t4File) {
    val text = Files.readString(findOutputFile(resultExtension))
      .replace("\\r\\n", "\\n").replace("\uFEFF", "")
    it.print(printer?.let { printer(text) } ?: text)
  }

  fun assertNoOutputWithExtension(extension: String) {
    val item = outputFileCandidates.firstOrNull {
      it.nameWithoutExtension == t4File.nameWithoutExtension && it.name.endsWith(extension)
    }
    assert(item == null) {
      "Execution should have failed, but seems to have succeeded. " +
        "File name: $item; File contents: ${Files.readString(item!!)}"
    }
  }
}