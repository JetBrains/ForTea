using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.Model;
using JetBrains.ForTea.RiderPlugin.ProtocolAware.Services;
using JetBrains.ForTea.RiderPlugin.Resources;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Protocol;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
  [Action(typeof(Strings), nameof(Strings.PreprocessTemplate_Text))]
  public sealed class T4PreprocessTemplateAction : T4FileBasedActionBase
  {
    public override void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      var solution = FindSolution(context).NotNull();
      var model = solution.GetProtocolSolution().GetT4ProtocolModel();
      var statistics = solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>();
      model.PreprocessingStarted();
      var file = FindT4File(context, solution).NotNull();
      statistics.TrackAction(T4StatisticIdBundle.PreprocessFromContextMenu);
      var preprocessingManager = solution.GetComponent<IT4TemplatePreprocessingManager>();
      model.PreprocessingFinished(preprocessingManager.Preprocess(file));
    }
  }
}