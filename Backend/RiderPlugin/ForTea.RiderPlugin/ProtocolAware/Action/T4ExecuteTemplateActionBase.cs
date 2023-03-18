using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
  public abstract class T4ExecuteTemplateActionBase : T4FileBasedActionBase
  {
    protected override bool DoUpdate(IPsiSourceFile file, ISolution solution) =>
      !solution.GetComponent<IT4TemplateExecutionManager>().IsExecutionRunning(file);

    public sealed override void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      var solution = FindSolution(context);
      if (solution == null) return;
      solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>().TrackAction(ActionId);
      var file = FindT4File(context, solution);
      if (file == null) return;
      var executionManager = solution.GetComponent<IT4TemplateExecutionManager>();
      // It is necessary to modify template kind
      // before execution because this affects
      // the context in which its assemblies are resolved
      executionManager.UpdateTemplateKind(file);
      Execute(executionManager, file);
    }

    protected abstract void Execute([NotNull] IT4TemplateExecutionManager executionManager, [NotNull] IT4File file);

    [NotNull] protected abstract string ActionId { get; }
  }
}