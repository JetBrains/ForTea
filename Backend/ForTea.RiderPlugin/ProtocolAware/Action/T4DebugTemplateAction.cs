using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ProjectModel;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	[Action("T4.DebugFromContext", "Debug Template")]
	public class T4DebugTemplateAction : T4FileBasedActionBase
	{
		public override void Execute(IDataContext context, DelegateExecute nextExecute)
		{
			var solution = FindSolution(context);
			if (solution == null) return;
			var file = FindT4File(context);
			if (file == null) return;
			var manager = solution.GetComponent<IT4TemplateExecutionManager>();
			if (!manager.CanExecute(file)) return;
			solution.GetLifetime().UsingNested(nested => manager.Debug(nested, file));
		}
	}
}
