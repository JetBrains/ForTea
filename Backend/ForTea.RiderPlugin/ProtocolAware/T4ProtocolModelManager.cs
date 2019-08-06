using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	[SolutionComponent]
	public sealed class T4ProtocolModelManager : GammaJul.ForTea.Core.ProtocolAware.Impl.T4ProtocolModelManager
	{
		[NotNull] private const string PreprocessResultExtension = "cs";

		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private T4ProtocolModel Model { get; }

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private T4DirectiveInfoManager DirectiveInfoManager { get; }

		[NotNull]
		private IT4TemplateExecutionManager ExecutionManager { get; }

		[NotNull]
		private IT4TargetFileManager TargetFileManager { get; }

		public T4ProtocolModelManager(
			[NotNull] ISolution solution,
			[NotNull] IT4TargetFileManager targetFileManager,
			[NotNull] IT4TemplateExecutionManager executionManager,
			[NotNull] ILogger logger, [NotNull] T4DirectiveInfoManager directiveInfoManager)
		{
			Solution = solution;
			TargetFileManager = targetFileManager;
			ExecutionManager = executionManager;
			Logger = logger;
			DirectiveInfoManager = directiveInfoManager;
			Model = solution.GetProtocolSolution().GetT4ProtocolModel();
			RegisterCallbacks();
		}

		private void RegisterCallbacks()
		{
			var defaultBuildMessage = new BuildMessage(T4BuildMessageKind.T4Error, "Internal error");
			var defaultBuildMessages = new List<BuildMessage>{defaultBuildMessage};
			var defaultBuildResult = new T4BuildResult(T4BuildResultKind.HasErrors, defaultBuildMessages);
			Model.RequestCompilation.Set(WrapClassFunc(Compile, defaultBuildResult));
			Model.TransferResults.Set(WrapClassFunc(CopyResults, Unit.Instance));
			Model.RequestPreprocessing.Set(WrapStructFunc(Preprocess, false));
		}

		public override void UpdateFileInfo(IT4File file) =>
			Model.Configurations[file.GetSourceFile().GetLocation().FullPath.Replace("\\", "/")] =
				new T4ConfigurationModel(
					TargetFileManager.GetTemporaryExecutableLocation(file).FullPath.Replace("\\", "/"),
					TargetFileManager.GetTemporaryTargetFileLocation(file).FullPath.Replace("\\", "/")
				);

		private Func<string, T> WrapClassFunc<T>(Func<IT4File, T> wrappee, [NotNull] T defaultValue) where T : class =>
			rawPath =>
			{
				var result = Logger.Catch(() =>
				{
					var path = FileSystemPath.Parse(rawPath);
					using (ReadLockCookie.Create())
					{
						var sourceFile = path.FindSourceFileInSolution(Solution);
						var t4File = sourceFile?.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleOrDefault();
						if (t4File == null) return defaultValue;
						return wrappee(t4File);
					}
				});
				return result ?? defaultValue;
			};

		private Func<string, T> WrapStructFunc<T>(Func<IT4File, T?> wrappee, T defaultValue) where T : struct =>
			rawPath =>
			{
				var result = Logger.Catch(() =>
				{
					var path = FileSystemPath.Parse(rawPath);
					using (ReadLockCookie.Create())
					{
						var sourceFile = path.FindSourceFileInSolution(Solution);
						var t4File = sourceFile?.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleOrDefault();
						if (t4File == null) return defaultValue;
						return wrappee(t4File);
					}
				});
				return result ?? defaultValue;
			};

		private T4BuildResult Compile([NotNull] IT4File t4File) =>
			ExecutionManager.Compile(Solution.GetLifetime(), t4File);

		[CanBeNull]
		private Unit CopyResults([NotNull] IT4File file)
		{
			var temporaryTargetFileLocation = TargetFileManager.GetTemporaryTargetFileLocation(file);
			Assertion.Assert(temporaryTargetFileLocation.ExistsFile, "temporaryTargetFileLocation.ExistsFile");
			TargetFileManager.SaveResults(new T4ExecutionResultInFile(temporaryTargetFileLocation), file);
			return Unit.Instance;
		}

		private bool? Preprocess([NotNull] IT4File file)
		{
			string message = new T4CSharpCodeGenerator(file, DirectiveInfoManager).Generate().RawText;
			using (WriteLockCookie.Create())
			{
				TargetFileManager.SaveResults(new T4ExecutionResultInString(message), file, PreprocessResultExtension);
			}

			return true;
		}
	}
}
