using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	public abstract class T4ExecuteTemplateActionBase : T4FileBasedActionBase
	{
		protected override bool DoUpdate(IPsiSourceFile file, ISolution solution) =>
			!solution.GetComponent<IT4TemplateExecutionManager>().IsExecutionRunning(file);

		public sealed override void Execute(IDataContext context, DelegateExecute nextExecute)
		{
			var solution = FindSolution(context);
			if (solution == null) return;
			var file = FindT4File(context, solution);
			if (file == null) return;
			var executionManager = solution.GetComponent<IT4TemplateExecutionManager>();
			var statistics = solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>();
			var dataManager = solution.GetComponent<IT4ProjectModelTemplateMetadataManager>();
			var projectFile = file.GetSourceFile().NotNull().ToProjectFile().NotNull();
			UpdateTemplateKind(solution, dataManager, projectFile, statistics);
			Execute(executionManager, file);
		}

		// It is necessary to modify template kind
		// before execution because this affects
		// the context in which its assemblies are resolved
		private void UpdateTemplateKind(
			[NotNull] ISolution solution,
			[NotNull] IT4ProjectModelTemplateMetadataManager dataManager,
			[NotNull] IProjectFile projectFile,
			[NotNull] Application.ActivityTrackingNew.UsageStatistics statistics
		)
		{
			solution.InvokeUnderTransaction(cookie =>
				dataManager.UpdateTemplateMetadata(cookie, projectFile, T4TemplateKind.Executable));
			statistics.TrackAction(ActionId);
			// Apply the changes, just in case the template kind was different
			solution.GetPsiServices().Files.CommitAllDocuments();
		}

		protected abstract void Execute([NotNull] IT4TemplateExecutionManager executionManager, [NotNull] IT4File file);

		[NotNull]
		protected abstract string ActionId { get; }
	}
}
