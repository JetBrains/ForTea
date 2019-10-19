using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing.Managing;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services.Impl
{
	[SolutionComponent]
	public sealed class T4ProjectModelTemplateMetadataManager : IT4ProjectModelTemplateMetadataManager
	{
		[NotNull]
		private IT4TargetFileChecker TargetFileChecker { get; }

		public T4ProjectModelTemplateMetadataManager([NotNull] IT4TargetFileChecker targetFileChecker) =>
			TargetFileChecker = targetFileChecker;

		public void UpdateTemplateMetadata(
			IProjectModelTransactionCookie cookie,
			IProjectFile template,
			T4TemplateKind kind,
			FileSystemPath outputLocation
		) => cookie.EditFileProperties(template, properties =>
		{
			if (!(properties is ProjectFileProperties projectFileProperties)) return;
			if (outputLocation != null) projectFileProperties.CustomToolOutput = outputLocation.Name;
			projectFileProperties.CustomTool = T4TemplateManagerConstants.ToRawValue(kind);
		});

		public void UpdateGeneratedFileMetadata(
			IProjectModelTransactionCookie cookie,
			IProjectFile generatedFile,
			IProjectFile template
		) => cookie.EditFileProperties(generatedFile, properties =>
		{
			if (!(properties is ProjectFileProperties projectFileProperties)) return;
			projectFileProperties.IsCustomToolOutput = true;
			projectFileProperties.IsDesignTimeBuildInput = true;
			projectFileProperties.DependsUponName = template.Name;
		});

		public IEnumerable<IProjectFile> FindLastGenOutput(IProjectFile file)
		{
			var properties = file.Properties as ProjectFileProperties;
			string targetFileName = properties?.CustomToolOutput;
			if (targetFileName == null) return Enumerable.Empty<IProjectFile>();
			var folder = file.ParentFolder;
			if (folder == null) return Enumerable.Empty<IProjectFile>();
			return folder
				.GetSubItems(targetFileName)
				.ToList()
				.OfType<IProjectFile>()
				.Where(suspect => TargetFileChecker.IsGeneratedFrom(suspect, file));
		}
	}
}
