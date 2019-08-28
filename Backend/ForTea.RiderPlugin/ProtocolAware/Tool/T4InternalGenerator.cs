using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Threading;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.Altering.Resources;
using JetBrains.ReSharper.Host.Features.ProjectModel.CustomTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.UI.Icons;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Tool
{
	[ShellComponent]
	public sealed class T4InternalGenerator : ISingleFileCustomTool
	{
		public string Name => "Bundled T4 template executor";
		public string ActionName => "Execute T4 generator";
		public IconId Icon => FileLayoutThemedIcons.TypeTemplate.Id;
		public string[] CustomTools => new[] {"T4 Generator Custom Tool"};
		public bool IsEnabled => true;

		public string[] Extensions => new[]
		{
			T4FileExtensions.MainExtensionNoDot,
			T4FileExtensions.SecondExtensionNoDot
			// .ttinclude files exist for being included and should not be auto-executed
		};

		public bool IsApplicable(IProjectFile projectFile)
		{
			using (projectFile.Locks.UsingReadLock())
			{
				return projectFile.LanguageType.Is<T4ProjectFileType>();
			}
		}

		public string[] Keywords => new[] {"tt", "t4", "template", "generator"};

		public ISingleFileCustomToolExecutionResult Execute(IProjectFile projectFile)
		{
			var file = projectFile.ToSourceFile()?.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleOrDefault();
			var manager = file?.GetSolution().GetComponent<IT4TemplateExecutionManager>();
			Execute(file, manager);
			// TODO: do we still need this to be ISingleFileCustomTool
			return new SingleFileCustomToolExecutionResult(
				EmptyList<FileSystemPath>.Collection,
				EmptyList<string>.Collection
			);
		}

		private static void Execute([CanBeNull] IT4File file, [CanBeNull] IT4TemplateExecutionManager manager)
		{
			if (file == null || manager == null) return;
			if (manager.IsExecutionRunning(file)) return;
			manager.ExecuteSilently(file);
		}
	}
}
