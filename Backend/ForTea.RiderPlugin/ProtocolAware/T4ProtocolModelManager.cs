using System;
using System.Linq;
using FluentAssertions;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Tool;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	[SolutionComponent]
	public sealed class T4ProtocolModelManager
	{
		[NotNull]
		private ILogger Logger { get; }

		[NotNull]
		private T4ProtocolModel Model { get; }

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private IT4TemplateExecutionManager ExecutionManager { get; }

		[NotNull]
		private IT4TargetFileManager TargetFileManager { get; }

		[NotNull]
		private IT4BuildMessageConverter Converter { get; }

		[NotNull]
		private ProjectModelViewHost Host { get; }

		public T4ProtocolModelManager(
			Lifetime lifetime,
			[NotNull] ISolution solution,
			[NotNull] IT4TargetFileManager targetFileManager,
			[NotNull] IT4TemplateExecutionManager executionManager,
			[NotNull] ILogger logger,
			[NotNull] T4BuildMessageConverter converter,
			[NotNull] ProjectModelViewHost host,
			[NotNull] T4InternalGenerator generator
		)
		{
			Solution = solution;
			TargetFileManager = targetFileManager;
			ExecutionManager = executionManager;
			Logger = logger;
			Converter = converter;
			Host = host;
			Model = solution.GetProtocolSolution().GetT4ProtocolModel();
			RegisterCallbacks(lifetime, generator);
		}

		private void RegisterCallbacks(Lifetime lifetime, [NotNull] T4InternalGenerator generator)
		{
			Model.RequestCompilation.Set(Wrap(Compile, Converter.FatalError()));
			Model.ExecutionSucceeded.Set(Wrap(HandleSuccess, Unit.Instance));
			Model.RequestPreprocessing.Set(Wrap(Preprocess, new T4PreprocessingResult(false, null)));
			Model.GetConfiguration.Set(Wrap(CalculateConfiguration, new T4ConfigurationModel("", "")));

			bool IsExecutionAllowed() => !Model.UserSessionActive.Maybe.ValueOrDefault;

			lifetime.Bracket(
				() => generator.ExecutionRequested += IsExecutionAllowed,
				() => generator.ExecutionRequested -= IsExecutionAllowed
			);
		}

		private T4ConfigurationModel CalculateConfiguration([NotNull] IT4File file) => new T4ConfigurationModel(
			TargetFileManager.GetTemporaryExecutableLocation(file).FullPath.Replace("\\", "/"),
			TargetFileManager.GetExpectedTemporaryTargetFileLocation(file).FullPath.Replace("\\", "/")
		);

		private Func<T4FileLocation, T> Wrap<T>(Func<IT4File, T> wrappee, [NotNull] T defaultValue) where T : class =>
			location =>
			{
				var result = Logger.Catch(() =>
				{
					var path = FileSystemPath.Parse(location.Location);
					if (path.IsNullOrEmpty()) return defaultValue;
					using (ReadLockCookie.Create())
					{
						var file = Host
							.GetItemById<IProject>(location.ProjectId)
							?.GetSubItemsRecursively(path.Name)
							.Where(it => it.Location == path)
							.AsList()
							.SingleItem()
							.As<IProjectFile>()
							?.ToSourceFile()
							?.GetPsiFiles(T4Language.Instance)
							.OfType<IT4File>()
							.SingleItem();
						return file == null ? null : wrappee(file);
					}
				});
				return result ?? defaultValue;
			};

		private T4BuildResult Compile([NotNull] IT4File t4File)
		{
			using (WriteLockCookie.Create())
			{
				// Interrupt template execution, if any
			}
			return ExecutionManager.Compile(Solution.GetLifetime(), t4File);
		}

		[CanBeNull]
		private Unit HandleSuccess([NotNull] IT4File file)
		{
			var destination = TargetFileManager.CopyExecutionResults(file);
			using (WriteLockCookie.Create())
			{
				TargetFileManager.UpdateProjectModel(file, destination);
			}

			return Unit.Instance;
		}

		[NotNull]
		private T4PreprocessingResult Preprocess([NotNull] IT4File file)
		{
			try
			{
				string message = new T4CSharpCodeGenerator(file, Solution).Generate().RawText;
				using (WriteLockCookie.Create())
				{
					TargetFileManager.SavePreprocessResults(file, message);
				}

				return new T4PreprocessingResult(true, null);
			}
			catch (T4OutputGenerationException e)
			{
				return Converter.ToT4PreprocessingResult(e);
			}
		}
	}
}
