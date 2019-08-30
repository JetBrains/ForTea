using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.UI.ActionsRevised.Menu;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	[Action("T4.DebugFromContext", "Debug Template")]
	public class T4DebugTemplateAction : T4ProtocolAwareActionBase
	{
		protected override void Execute(
			IT4TemplateExecutionManager manager,
			IT4File file,
			Application.ActivityTrackingNew.UsageStatistics statistics
		)
		{
			statistics.TrackAction("T4.Template.Debug");
			manager.Debug(file);
		}
	}
}
