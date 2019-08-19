using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.Altering.Resources;
using JetBrains.ReSharper.Host.Features.ProjectModel.CustomTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.UI.Icons;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Tool
{
	[ShellComponent]
	public sealed class T4InternalGenerator : ISingleFileCustomTool
	{
		private Lifetime Lifetime { get; }

		public T4InternalGenerator(Lifetime lifetime) => Lifetime = lifetime;

		public string Name => "Bundled T4 template executor";
		public string ActionName => "Execute T4 generator";
		public IconId Icon => FileLayoutThemedIcons.TypeTemplate.Id;
		public string[] CustomTools => new[] {"T4 Generator Custom Tool"};
		public string[] Extensions => new[] {T4ProjectFileType.MainExtensionNoDot};
		public bool IsEnabled => true;

		public bool IsApplicable(IProjectFile projectFile)
		{
			using (projectFile.Locks.UsingReadLock())
			{
				// .ttinclude and .t4 files exist for being included and should not be auto-executed
				if (projectFile.Location.ExtensionWithDot != T4ProjectFileType.MainExtension) return false;
				return projectFile.LanguageType.Is<T4ProjectFileType>();
			}
		}

		public string[] Keywords => new[] {"tt", "t4", "template", "generator"};

		public ISingleFileCustomToolExecutionResult Execute(IProjectFile projectFile)
		{
			AssertOperationValidity(projectFile);
			var file = AsT4File(projectFile).NotNull();
			var solution = file.GetSolution();
			var executionManager = solution.GetComponent<IT4TemplateExecutionManager>();
			var targetFileManager = solution.GetComponent<IT4TargetFileManager>();
			InterruptableActivityCookie.CheckAndThrow();
			if (!executionManager.CanCompile(file))
			{
				return new SingleFileCustomToolExecutionResult(
					new FileSystemPath[] { },
					new[] {"File contains syntax errors"}
				);
			}

			InterruptableActivityCookie.CheckAndThrow();
			var buildResult = executionManager.Compile(Lifetime, file);
			Assertion.Assert(buildResult.BuildResultKind != T4BuildResultKind.HasErrors,
				"buildResult.BuildResultKind != T4BuildResultKind.HasErrors");
			InterruptableActivityCookie.CheckAndThrow();
			bool succeeded = executionManager.Execute(Lifetime, file);
			if (!succeeded)
				return new SingleFileCustomToolExecutionResult(
					EmptyList<FileSystemPath>.Collection,
					new List<string> {"Execution error"});

			InterruptableActivityCookie.CheckAndThrow();
			var affectedFile = targetFileManager.CopyExecutionResults(file);
			solution.Locks.ExecuteOrQueueEx(solution.GetLifetime(), "Saving T4 results", () =>
			{
				using (WriteLockCookie.Create())
				{
					targetFileManager.UpdateProjectModel(file, affectedFile);
				}
			});

			return new SingleFileCustomToolExecutionResult(new[] {affectedFile}, EmptyList<string>.Collection);
		}

		private static void AssertOperationValidity([NotNull] IProjectFile projectFile)
		{
			projectFile.AssertIsValid();
			projectFile.Locks.AssertReadAccessAllowed();
			projectFile.GetSolution().GetPsiServices().Files.AssertAllDocumentAreCommitted();
		}

		[CanBeNull]
		private static IT4File AsT4File([CanBeNull] IProjectFile projectFile) =>
			projectFile?.ToSourceFile()?.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleOrDefault();
	}
}
