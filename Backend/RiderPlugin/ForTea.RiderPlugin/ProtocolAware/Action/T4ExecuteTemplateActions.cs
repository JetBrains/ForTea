using GammaJul.ForTea.Core.Tree;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ForTea.RiderPlugin.Resources;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
  [Action(T4ActionIdBundle.Run, nameof(Strings.RunTemplate_Text))]
  public sealed class T4ExecuteTemplateAction : T4ExecuteTemplateActionBase
  {
    protected override void Execute(IT4TemplateExecutionManager executionManager, IT4File file) =>
      executionManager.Execute(file);

    protected override string ActionId => T4StatisticIdBundle.RunFromContextMenu;
  }

  [Action(T4ActionIdBundle.Debug, nameof(Strings.DebugTemplate_Text))]
  public sealed class T4DebugTemplateAction : T4ExecuteTemplateActionBase
  {
    protected override void Execute(IT4TemplateExecutionManager executionManager, IT4File file) =>
      executionManager.Debug(file);

    protected override string ActionId => T4StatisticIdBundle.DebugFromContextMenu;
  }
}