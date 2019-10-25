using System;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services.Impl
{
	[SolutionComponent]
	public class T4TemplatePreprocessingManager : IT4TemplatePreprocessingManager
	{
		private DateTime PreviousExecutedFileWriteTime { get; set; }

		[NotNull]
		private IT4TargetFileManager TargetFileManager { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4TemplatePreprocessingManager(
			[NotNull] IT4TargetFileManager targetFileManager,
			[NotNull] ILogger logger
		)
		{
			TargetFileManager = targetFileManager;
			Logger = logger;
		}

		public void Preprocess(IT4File file)
		{
			Logger.Verbose("Preprocessing a file");
			var psiSourceFile = file.GetSourceFile();
			if (psiSourceFile == null) return;
			var lastWriteTimeUtc = psiSourceFile.LastWriteTimeUtc;
			if (lastWriteTimeUtc == PreviousExecutedFileWriteTime) return;
			PreviousExecutedFileWriteTime = lastWriteTimeUtc;
			var solution = file.GetSolution();
			string message = new T4CSharpCodeGenerator(file, solution).Generate().RawText;
			solution.Locks.ExecuteOrQueueEx(solution.GetLifetime(), "T4 template preprocessing", () =>
			{
				using (WriteLockCookie.Create())
				{
					TargetFileManager.SavePreprocessResults(file, message);
				}
			});
		}
	}
}
