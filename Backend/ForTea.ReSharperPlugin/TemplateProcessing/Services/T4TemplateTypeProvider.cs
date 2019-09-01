using System;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;

namespace JetBrains.ForTea.ReSharperPlugin.TemplateProcessing.Services
{
	[SolutionComponent]
	public class T4TemplateTypeProvider : IT4TemplateTypeProvider
	{
		public TemplateKind GetTemplateKind(IProjectFile file)
		{
			var properties = file?.Properties as ProjectFileProperties;
			string customTool = properties?.CustomTool;
			if ("TextTemplatingFilePreprocessor".Equals(customTool, StringComparison.OrdinalIgnoreCase))
				return TemplateKind.Preprocessed;
			if ("TextTemplatingFileGenerator".Equals(customTool, StringComparison.OrdinalIgnoreCase))
				return TemplateKind.Executable;
			return TemplateKind.Unknown;
		}
	}
}
