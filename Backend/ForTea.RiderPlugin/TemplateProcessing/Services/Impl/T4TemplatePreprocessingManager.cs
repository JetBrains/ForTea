using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.Util;
using JetBrains.Util.dataStructures;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services.Impl
{
	[SolutionComponent]
	public sealed class T4TemplatePreprocessingManager : IT4TemplatePreprocessingManager
	{
		private DateTime PreviousExecutedFileWriteTime { get; set; }

		[NotNull]
		private IT4TargetFileManager TargetFileManager { get; }

		[NotNull]
		private IT4BuildMessageConverter BuildMessageConverter { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4TemplatePreprocessingManager(
			[NotNull] IT4TargetFileManager targetFileManager,
			[NotNull] ILogger logger,
			[NotNull] IT4BuildMessageConverter buildMessageConverter
		)
		{
			TargetFileManager = targetFileManager;
			Logger = logger;
			BuildMessageConverter = buildMessageConverter;
		}

		public void TryPreprocess(IT4File file)
		{
			Logger.Verbose("Trying to preprocess a file");
			var psiSourceFile = file.PhysicalPsiSourceFile.NotNull();
			var lastWriteTimeUtc = psiSourceFile.LastWriteTimeUtc;
			if (lastWriteTimeUtc == PreviousExecutedFileWriteTime) return;
			PreviousExecutedFileWriteTime = lastWriteTimeUtc;
			Preprocess(file);
		}

		public T4PreprocessingResult Preprocess(IT4File file)
		{
			Logger.Verbose("Preprocessing a file");
			var psiSourceFile = file.PhysicalPsiSourceFile.NotNull();
			var projectFile = psiSourceFile.ToProjectFile().NotNull();
			var solution = file.GetSolution();
			var contextFreeTree = psiSourceFile.BuildT4Tree();
			var location = new T4FileLocation(solution.GetComponent<ProjectModelViewHost>().GetIdByItem(projectFile));
			try
			{
				string message = new T4CSharpPreprocessedCodeGenerator(contextFreeTree, solution).Generate().RawText;
				solution.Locks.ExecuteOrQueueEx(solution.GetLifetime(), "T4 template preprocessing", () =>
				{
					using (WriteLockCookie.Create())
					{
						TargetFileManager.SavePreprocessResults(file, message);
					}
				});
				return new T4PreprocessingResult(location, true, new List<T4BuildMessage>());
			}
			catch (T4OutputGenerationException e)
			{
				var message = BuildMessageConverter.ToT4BuildMessages(e.FailureDatum.AsEnumerable());
				return new T4PreprocessingResult(location, false, message);
			}
		}
	}
}
