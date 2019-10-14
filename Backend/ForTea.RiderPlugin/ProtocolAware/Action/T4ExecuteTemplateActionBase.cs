using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;
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
			var dataManager = solution.GetComponent<IT4ProjectModelTemplateDataManager>();
			var projectFile = file.GetSourceFile().NotNull().ToProjectFile().NotNull();
			dataManager.SetTemplateKind(projectFile, T4TemplateKind.Executable);
			statistics.TrackAction(ActionId);
			// Template kind might have changed
			solution.GetPsiServices().Files.CommitAllDocuments();
			Execute(executionManager, file);
		}

		protected abstract void Execute([NotNull] IT4TemplateExecutionManager executionManager, [NotNull] IT4File file);

		[NotNull]
		protected abstract string ActionId { get; }
	}
}
