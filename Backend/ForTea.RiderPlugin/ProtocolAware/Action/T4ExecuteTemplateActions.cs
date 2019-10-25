using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.UI.ActionsRevised.Menu;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	[Action("T4.ExecuteFromContext", "Execute Template")]
	public sealed class T4ExecuteTemplateAction : T4ExecuteTemplateActionBase
	{
		protected override void Execute(IT4TemplateExecutionManager executionManager, IT4File file) =>
			executionManager.Execute(file);

		protected override string ActionId => "T4.Template.Execute";
	}

	[Action("T4.DebugFromContext", "Debug Template")]
	public sealed class T4DebugTemplateAction : T4ExecuteTemplateActionBase
	{
		protected override void Execute(IT4TemplateExecutionManager executionManager, IT4File file) =>
			executionManager.Debug(file);

		protected override string ActionId => "T4.Template.Debug";
	}
}
