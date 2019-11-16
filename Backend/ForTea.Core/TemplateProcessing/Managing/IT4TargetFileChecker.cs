using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Managing
{
	public interface IT4TargetFileChecker
	{
		bool IsPreprocessResult([NotNull] IProjectFile suspect);
		bool IsGenerationResult([NotNull] IProjectFile suspect);
		bool IsGeneratedFrom([NotNull] IProjectFile generated, [NotNull] IProjectFile source);
	}
}
