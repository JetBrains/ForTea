using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public static class T4TemplateDataManagerExtensions
	{
		public static bool IsPreprocessedTemplate(
			[NotNull] this IT4TemplateKindProvider manager,
			[NotNull] IProjectFile file
		) => manager.GetTemplateKind(file) == T4TemplateKind.Preprocessed;
	}
}
