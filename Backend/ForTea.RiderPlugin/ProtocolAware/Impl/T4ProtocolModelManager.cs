using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
	[SolutionComponent]
	public sealed class T4ProtocolModelManager
	{
		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private IT4TemplateCompiler Compiler { get; }

		[NotNull]
		private IT4TargetFileManager TargetFileManager { get; }

		[NotNull]
		private IT4BuildMessageConverter Converter { get; }

		[NotNull]
		private IT4TemplateExecutionManager ExecutionManager { get; }

		public T4ProtocolModelManager(
			[NotNull] ISolution solution,
			[NotNull] IT4TargetFileManager targetFileManager,
			[NotNull] IT4TemplateCompiler compiler,
			[NotNull] T4BuildMessageConverter converter,
			IT4ModelInteractionHelper helper,
			[NotNull] IT4TemplateExecutionManager executionManager
		)
		{
			Solution = solution;
			TargetFileManager = targetFileManager;
			Compiler = compiler;
			Converter = converter;
			ExecutionManager = executionManager;
			var model = solution.GetProtocolSolution().GetT4ProtocolModel();
			RegisterCallbacks(model, helper);
		}

		private void RegisterCallbacks([NotNull] T4ProtocolModel model, [NotNull] IT4ModelInteractionHelper helper)
		{
			model.RequestCompilation.Set(helper.Wrap(Compile, Converter.FatalError()));
			model.GetConfiguration.Set(helper.Wrap(CalculateConfiguration, new T4ConfigurationModel("", "")));
			model.ExecutionSucceeded.Set(helper.Wrap(HandleSuccess, Unit.Instance));
			model.ExecutionFailed.Set(helper.Wrap(HandleFailure, Unit.Instance));
			model.CanExecute.Set(helper.WrapStructFunc(CanExecute, false));
		}

		private T4ConfigurationModel CalculateConfiguration([NotNull] IT4File file) => new T4ConfigurationModel(
			TargetFileManager.GetTemporaryExecutableLocation(file).FullPath.Replace("\\", "/"),
			TargetFileManager.GetExpectedTemporaryTargetFileLocation(file).FullPath.Replace("\\", "/")
		);

		private T4BuildResult Compile([NotNull] IT4File t4File) => Compiler.Compile(Solution.GetLifetime(), t4File);

		[CanBeNull]
		private Unit HandleSuccess([NotNull] IT4File file)
		{
			var destination = TargetFileManager.CopyExecutionResults(file);
			using (WriteLockCookie.Create())
			{
				TargetFileManager.UpdateProjectModel(file, destination);
			}

			ExecutionManager.OnExecutionFinished(file);
			return Unit.Instance;
		}

		private Unit HandleFailure([NotNull] IT4File file)
		{
			ExecutionManager.OnExecutionFinished(file);
			return Unit.Instance;
		}

		private bool? CanExecute([NotNull] IT4File file) => !ExecutionManager.IsExecutionRunning(file);
	}
}
