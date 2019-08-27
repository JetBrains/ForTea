using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.UI.ActionsRevised.Menu;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	[Action("T4.ExecuteFromContext", "Execute Template")]
	public class T4ExecuteTemplateAction : T4ProtocolAwareActionBase
	{
		protected override void Execute(IT4TemplateExecutionManager manager, IT4File file) => manager.Execute(file);
	}
}
