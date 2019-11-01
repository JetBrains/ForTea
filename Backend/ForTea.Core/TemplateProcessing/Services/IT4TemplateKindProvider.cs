using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public interface IT4TemplateKindProvider
	{
		T4TemplateKind GetTemplateKind([NotNull] IProjectFile file);
		T4TemplateKind GetRootTemplateKind([NotNull] IProjectFile file);
	}
}
