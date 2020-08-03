using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing;
using JetBrains.ProjectModel;
using JetBrains.Rd.Tasks;
using JetBrains.ReSharper.Host.Features;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
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
		private IT4ProjectReferenceResolver ProjectReferenceResolver { get; }

		[NotNull]
		private ProjectModelViewHost Host { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4ProtocolModelManager(
			[NotNull] ISolution solution,
			[NotNull] IT4TargetFileManager targetFileManager,
			[NotNull] IT4TemplateCompiler compiler,
			[NotNull] IT4BuildMessageConverter converter,
			[NotNull] IT4ModelInteractionHelper helper,
			[NotNull] IT4TemplateExecutionManager executionManager,
			[NotNull] ILogger logger,
			[NotNull] ProjectModelViewHost host,
			[NotNull] IT4ProjectReferenceResolver projectReferenceResolver
		)
		{
			Solution = solution;
			TargetFileManager = targetFileManager;
			Compiler = compiler;
			Converter = converter;
			ExecutionManager = executionManager;
			Logger = logger;
			ProjectReferenceResolver = projectReferenceResolver;
			Host = host;
			var model = solution.GetProtocolSolution().GetT4ProtocolModel();
			RegisterCallbacks(model, helper);
		}

		private void RegisterCallbacks([NotNull] T4ProtocolModel model, [NotNull] IT4ModelInteractionHelper helper)
		{
			model.RequestCompilation.Set(helper.Wrap(Compile, Converter.FatalError()));
			model.GetConfiguration.Set(helper.Wrap(CalculateConfiguration, new T4ConfigurationModel("", "", 0)));
			model.GetProjectDependencies.Set(helper.Wrap(CalculateProjectDependencies, new List<int>()));
			model.ExecutionSucceeded.Set(helper.Wrap(ExecutionSucceeded));
			model.ExecutionFailed.Set(helper.Wrap(ExecutionFailed));
			model.ExecutionAborted.Set(helper.Wrap(ExecutionFailed));
			model.PrepareExecution.Set(helper.Wrap(PrepareExecution));
		}

		[CanBeNull]
		private List<int> CalculateProjectDependencies([NotNull] IPsiSourceFile file) => ProjectReferenceResolver
			.GetProjectDependencies(file.BuildT4Tree())
			.Select(it => Host.GetIdByProjectModelElement(it))
			.AsList();

		[NotNull]
		private T4ConfigurationModel CalculateConfiguration([NotNull] IT4File file) => new T4ConfigurationModel(
			TargetFileManager.GetTemporaryExecutableLocation(file).FullPath.Replace("\\", "/"),
			TargetFileManager.GetExpectedTemporaryTargetFileLocation(file).FullPath.Replace("\\", "/"),
			ExecutionManager.GetEnvDTEPort(file)
		);

		private T4BuildResult Compile([NotNull] IPsiSourceFile sourceFile)
		{
			T4BuildResult result = null;
			Solution.GetLifetime().UsingNested(nested => result = Compiler.Compile(nested, sourceFile));
			return result;
		}

		// When execution is said to succeed, we still cannot be sure whether temporary file exists or not.
		// If the process being executed was the debugger process,
		// it would exit normally even if the process it was debugging
		// (i.e. the generated transformation process) crashed
		private void ExecutionSucceeded([NotNull] IT4File file)
		{
			Logger.Verbose("Execution of a file succeeded");
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

		private void ExecutionFailed([NotNull] IT4File file)
		{
			Logger.Verbose("Execution of a file failed");
			ExecutionManager.OnExecutionFinished(file);
		}

		private void PrepareExecution([NotNull] IT4File file)
		{
			ExecutionManager.RememberExecution(file, false);
		}
	}
}
