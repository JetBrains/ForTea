using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.Resources;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.Altering.Resources;
using JetBrains.ReSharper.Host.Features.ProjectModel.CustomTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.UI.Icons;
using JetBrains.Util;
using UsageStatisticsNew = JetBrains.Application.ActivityTrackingNew.UsageStatistics;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Tool
{
	[ShellComponent] // This class cannot be made a solution component
	public sealed class T4InternalGenerator : ISingleFileCustomTool
	{
		public string Name => "Bundled T4 template executor";
		public string ActionName => null;
		public IconId Icon => FileLayoutThemedIcons.TypeTemplate.Id;
		public string[] CustomTools => new[] {"T4 Generator Custom Tool"};
		public bool IsEnabled => true;

		public string[] Extensions => new[]
		{
			T4FileExtensions.MainExtensionNoDot,
			T4FileExtensions.SecondExtensionNoDot
			// .ttinclude files exist for being included and should not be auto-executed
		};

		[NotNull]
		private UsageStatisticsNew Statistics { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4InternalGenerator([NotNull] UsageStatisticsNew statistics, [NotNull] ILogger logger)
		{
			Statistics = statistics;
			Logger = logger;
		}

		public bool IsApplicable(IProjectFile projectFile)
		{
			using (projectFile.Locks.UsingReadLock())
			{
				if (!projectFile.LanguageType.Is<T4ProjectFileType>()) return false;
				var dataManager = projectFile.GetSolution().GetComponent<IT4TemplateKindProvider>();
				var kind = dataManager.GetTemplateKind(projectFile);
				return kind != T4TemplateKind.Unknown;
			}
		}

		public string[] Keywords => new[] {"tt", "t4", "template", "generator"};

		[NotNull]
		public ISingleFileCustomToolExecutionResult Execute(IProjectFile projectFile)
		{
			Logger.Verbose("Got request to execute a file");
			var file = projectFile.ToSourceFile()?.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleOrDefault();
			if (file == null) return SingleFileCustomToolExecutionResult.NotExecuted;
			var solution = file.GetSolution();
			var dataManager = solution.GetComponent<IT4TemplateKindProvider>();
			var kind = dataManager.GetTemplateKind(projectFile);
			if (kind is T4TemplateKind.Executable) Execute(file, solution);
			if (kind is T4TemplateKind.Preprocessed) Preprocess(file, solution);
			return SingleFileCustomToolExecutionResult.NotExecuted;
		}

		private void Execute([NotNull] IT4File file, [NotNull] ISolution solution)
		{
			Statistics.TrackAction(T4StatisticIdBundle.RunSilently);
			var manager = solution.GetComponent<IT4TemplateExecutionManager>();
			if (manager.IsExecutionRunning(file.GetSourceFile().NotNull())) return;
			manager.ExecuteSilently(file);
		}

		private void Preprocess([NotNull] IT4File file, [NotNull] ISolution solution)
		{
			Statistics.TrackAction(T4StatisticIdBundle.PreprocessSilently);
			solution.GetComponent<IT4TemplatePreprocessingManager>().Preprocess(file);
		}
	}
}
