package kotlin.com.jetbrains.fortea.actions

import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.project.DumbService
import com.intellij.openapi.project.Project
import com.intellij.psi.search.FileTypeIndex
import com.intellij.psi.search.GlobalSearchScope
import com.jetbrains.fortea.language.T4FileType
import com.jetbrains.rider.projectView.views.solutionExplorer.SolutionExplorerCustomization

class T4SolutionExplorerCustomization(project: Project): SolutionExplorerCustomization(project) {
  override fun getNonImportantActionsForAddGroup(e: AnActionEvent) =
    if (DumbService.isDumb(project) || !FileTypeIndex.containsFileOfType(T4FileType, GlobalSearchScope.projectScope(project))) listOf("NewT4File")
    else super.getNonImportantActionsForAddGroup(e)
}