using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public static class T4TemplateTypeProviderExtensions
	{
		public static bool IsPreprocessedTemplate(
			[NotNull] this IT4TemplateTypeProvider provider,
			[NotNull] IProjectFile file
		) => provider.GetTemplateKind(file) == TemplateKind.Preprocessed;
	}
}
