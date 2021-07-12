using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public sealed class T4TargetFileChecker : IT4TargetFileChecker
	{
		public bool IsPreprocessResult(IProjectFile suspect) => IsActionResult(suspect, T4TemplateKind.Preprocessed);
		public bool IsGenerationResult(IProjectFile suspect) => IsActionResult(suspect, T4TemplateKind.Executable);

		public bool IsGeneratedFrom(IProjectFile generated, IProjectFile source)
		{
			// cannot check AutoGen and DesignTime, because they are not added in .NET Core projects
			if (!(generated.Properties is ProjectFileProperties generatedProperties)) return false;
			return generatedProperties.DependsUponName == source.Name;
		}

		private bool IsActionResult([NotNull] IProjectFile suspect, T4TemplateKind kind)
		{
			if (!(suspect.Properties is ProjectFileProperties properties)) return false;
			string sourceName = properties.DependsUponName;
			if (sourceName.IsEmpty()) return false;
			var parentFolder = suspect.ParentFolder;
			if (parentFolder == null) return false;
			return parentFolder
				.GetSubItems(sourceName)
				.ToList()
				.OfType<IProjectFile>()
				.Any(source => IsActionResult(suspect, source, kind));
		}

		private bool IsActionResult(
			[NotNull] IProjectFile generated,
			[NotNull] IProjectFile source,
			T4TemplateKind kind
		)
		{
			if (!(source.Properties is ProjectFileProperties sourceProperties)) return false;
			var templateKind = T4TemplateManagerConstants.ToTemplateKind(sourceProperties.CustomTool);
			if (templateKind != kind) return false;
			if (sourceProperties.CustomToolOutput != generated.Name) return false;
			if (!IsGeneratedFrom(generated, source)) return false;
			return true;
		}
	}
}
