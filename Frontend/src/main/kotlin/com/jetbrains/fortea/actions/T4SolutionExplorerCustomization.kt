package com.jetbrains.fortea.actions

import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.project.DumbService
import com.intellij.openapi.project.Project
import com.intellij.psi.search.FileTypeIndex
import com.jetbrains.fortea.language.T4FileType
import com.jetbrains.rider.ideaInterop.find.scopes.RiderSingleProjectModelEntityScope
import com.jetbrains.rider.projectView.views.solutionExplorer.SolutionExplorerCustomization
import com.jetbrains.rider.projectView.workspace.containingProjectEntity
import com.jetbrains.rider.projectView.workspace.getProjectModelEntity

class T4SolutionExplorerCustomization(project: Project): SolutionExplorerCustomization(project) {
  override fun getNonImportantActionsForAddGroup(e: AnActionEvent) =
    if (shouldShowT4Template(e)) super.getNonImportantActionsForAddGroup(e)
    else listOf("NewT4File")

  private fun shouldShowT4Template(e: AnActionEvent): Boolean {
    if (DumbService.isDumb(project)) return false
    val entity = e.dataContext.getProjectModelEntity(false)?.containingProjectEntity() ?: return false
    val scope = RiderSingleProjectModelEntityScope(project, entity, true)
    return FileTypeIndex.containsFileOfType(T4FileType, scope)
  }
}