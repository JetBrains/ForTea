using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services.Impl
{
	[SolutionComponent]
	public sealed class T4TemplateKindProvider : IT4TemplateKindProvider
	{
		public T4TemplateKind GetTemplateKind(IPsiSourceFile file) => GetTemplateKind(file?.ToProjectFile());

		public T4TemplateKind GetTemplateKind(IProjectFile file)
		{
			var properties = file?.Properties as ProjectFileProperties;
			string customTool = properties?.CustomTool;
			return T4TemplateManagerConstants.ToTemplateKind(customTool);
		}
	}
}
