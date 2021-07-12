using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public interface IT4TemplateKindProvider
	{
		T4TemplateKind GetTemplateKind([CanBeNull] IPsiSourceFile file);
		T4TemplateKind GetTemplateKind([CanBeNull] IProjectFile file);
	}
}
