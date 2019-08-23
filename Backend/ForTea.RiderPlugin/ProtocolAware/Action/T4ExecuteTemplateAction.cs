using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	[Action("T4.ExecuteFromContext", "Execute Template")]
	public class T4ExecuteTemplateAction : T4FileBasedActionBase
	{
		public override void Execute(IDataContext context, DelegateExecute nextExecute)
		{
		}
	}
}
