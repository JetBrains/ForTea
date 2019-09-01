using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public interface IT4TemplateTypeProvider
	{
		TemplateKind GetTemplateKind([NotNull] IProjectFile file);
	}
}
