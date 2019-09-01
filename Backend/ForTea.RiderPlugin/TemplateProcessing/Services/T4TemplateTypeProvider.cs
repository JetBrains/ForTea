using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services
{
	[SolutionComponent]
	public class T4TemplateTypeProvider : IT4TemplateTypeProvider
	{
		public TemplateKind GetTemplateKind(IProjectFile file) => TemplateKind.Unknown;
	}
}
