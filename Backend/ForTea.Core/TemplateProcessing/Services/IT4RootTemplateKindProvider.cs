using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public interface IT4RootTemplateKindProvider
	{
		T4TemplateKind GetRootTemplateKind([NotNull] IProjectFile file);
	}
}
