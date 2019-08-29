using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.Util;

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

		[NotNull]
		private ILogger Logger { get; }

		public T4ProtocolModelManager(
			Lifetime lifetime,
			[NotNull] ISolution solution,
			[NotNull] IT4TargetFileManager targetFileManager,
			[NotNull] IT4TemplateCompiler compiler,
			[NotNull] T4BuildMessageConverter converter,
			[NotNull] IT4ModelInteractionHelper helper,
			[NotNull] IT4TemplateExecutionManager executionManager,
			[NotNull] ILogger logger
		)
		{
			Solution = solution;
			TargetFileManager = targetFileManager;
			Compiler = compiler;
			Converter = converter;
			ExecutionManager = executionManager;
			Logger = logger;
			var model = solution.GetProtocolSolution().GetT4ProtocolModel();
			RegisterCallbacks(lifetime, model, helper);
		}

		private void RegisterCallbacks(
			Lifetime lifetime,
			[NotNull] T4ProtocolModel model,
			[NotNull] IT4ModelInteractionHelper helper
		)
		{
			model.RequestCompilation.Set(helper.Wrap(Compile, Converter.FatalError()));
			model.GetConfiguration.Set(helper.Wrap(CalculateConfiguration, new T4ConfigurationModel("", "")));
			model.ExecutionSucceeded.Advise(lifetime, helper.Wrap(ExecutionSucceeded));
			model.ExecutionFailed.Advise(lifetime, helper.Wrap(ExecutionFailed));
			model.ExecutionAborted.Advise(lifetime, helper.Wrap(ExecutionFailed));
		}

		[NotNull]
		private T4ConfigurationModel CalculateConfiguration([NotNull] IT4File file) => new T4ConfigurationModel(
			TargetFileManager.GetTemporaryExecutableLocation(file).FullPath.Replace("\\", "/"),
			TargetFileManager.GetExpectedTemporaryTargetFileLocation(file).FullPath.Replace("\\", "/")
		);

		private T4BuildResult Compile([NotNull] IT4File t4File) => Compiler.Compile(Solution.GetLifetime(), t4File);

		// When execution is said to succeed, we still cannot be sure whether temporary file exists or not.
		// If the process being executed was the debugger process,
		// it would exit normally even if the process it was debugging
		// (i.e. the generated transformation process) crashed
		private void ExecutionSucceeded([NotNull] IT4File file)
		{
			Logger.Catch(() =>
			{
				// This call is not expected to fail, but just in case
				using (WriteLockCookie.Create())
				{
					TargetFileManager.TryProcessExecutionResults(file);
				}
			});
			ExecutionManager.OnExecutionFinished(file);
		}

		private void ExecutionFailed([NotNull] IT4File file) => ExecutionManager.OnExecutionFinished(file);
	}
}
