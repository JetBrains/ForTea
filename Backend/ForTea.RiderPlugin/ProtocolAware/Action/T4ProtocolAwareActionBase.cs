using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	public abstract class T4ProtocolAwareActionBase : T4FileBasedActionBase
	{
		protected override bool DoUpdate(IT4File file, ISolution solution)
		{
			var executionManager = solution.GetComponent<IT4TemplateExecutionManager>();
			return !executionManager.IsExecutionRunning(file);
		}

		public override void Execute(IDataContext context, DelegateExecute nextExecute)
		{
			var solution = FindSolution(context);
			if (solution == null) return;
			var file = FindT4File(context);
			if (file == null) return;
			var executionManager = solution.GetComponent<IT4TemplateExecutionManager>();
			var statistics = solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>();
			var dataManager = solution.GetComponent<IT4ProjectModelTemplateDataManager>();
			Execute(executionManager, file, statistics, dataManager);
		}

		protected abstract void Execute(
			[NotNull] IT4TemplateExecutionManager executionManager,
			[NotNull] IT4File file,
			[NotNull] Application.ActivityTrackingNew.UsageStatistics statistics,
			[NotNull] IT4ProjectModelTemplateDataManager dataManager);
	}
}
