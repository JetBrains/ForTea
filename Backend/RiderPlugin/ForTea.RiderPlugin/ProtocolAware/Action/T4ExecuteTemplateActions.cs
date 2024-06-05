using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ForTea.RiderPlugin.Resources;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
  [Action(typeof(Strings), nameof(Strings.RunTemplate_Text))]
  public sealed class T4ExecuteTemplateAction : T4ExecuteTemplateActionBase
  {
    protected override void Execute(IT4TemplateExecutionManager executionManager, IT4File file) =>
      executionManager.Execute(file);

    protected override string ActionId => T4StatisticIdBundle.RunFromContextMenu;
  }

  [Action(typeof(Strings), nameof(Strings.DebugTemplate_Text))]
  public sealed class T4DebugTemplateAction : T4ExecuteTemplateActionBase
  {
    protected override void Execute(IT4TemplateExecutionManager executionManager, IT4File file) =>
      executionManager.Debug(file);

    protected override string ActionId => T4StatisticIdBundle.DebugFromContextMenu;
  }
}