using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	public abstract class T4ProtocolAwareActionBase : T4FileBasedActionBase
	{
		public override void Execute(IDataContext context, DelegateExecute nextExecute)
		{
			var solution = FindSolution(context);
			if (solution == null) return;
			var file = FindT4File(context);
			if (file == null) return;
			var manager = solution.GetComponent<IT4TemplateExecutionManager>();
			var statistics = solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>();
			Execute(manager, file, statistics);
		}

		protected abstract void Execute(
			[NotNull] IT4TemplateExecutionManager manager,
			[NotNull] IT4File file,
			[NotNull] Application.ActivityTrackingNew.UsageStatistics statistics);
	}
}
