using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.Model;
using JetBrains.ForTea.RiderPlugin.ProtocolAware.Services;
using JetBrains.ForTea.RiderPlugin.Resources;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Protocol;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;
using JetBrains.UI.ThemedIcons;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.RunMarkers
{
  public sealed class T4FileRunMarkerGutterMark : IconGutterMarkType
  {
    public T4FileRunMarkerGutterMark() : base(RunMarkersThemedIcons.RunActions.Id)
    {
    }

    [ItemNotNull]
    public override IEnumerable<BulbMenuItem> GetBulbMenuItems(IHighlighter highlighter)
    {
      if (!(highlighter.UserData is T4RunMarkerHighlighting highlighting)) yield break;
      var directive = highlighting.Directive;
      yield return new BulbMenuItem(
        CreateRunFileExecutableItem(
          directive,
          (manager, file) => manager.Execute(file),
          T4StatisticIdBundle.RunFromGutter
        ),
        new RichText(Strings.RunTemplate_Text),
        RunMarkersThemedIcons.RunThis.Id,
        BulbMenuAnchors.PermanentBackgroundItems);
      yield return new BulbMenuItem(
        CreateRunFileExecutableItem(
          directive,
          (manager, file) => manager.Debug(file),
          T4StatisticIdBundle.DebugFromGutter
        ),
        new RichText(Strings.DebugTemplate_Text),
        RunMarkersThemedIcons.DebugThis.Id,
        BulbMenuAnchors.PermanentBackgroundItems
      );
      yield return new BulbMenuItem(
        new ExecutableItem(() =>
        {
          var file = (IT4File)directive.GetContainingFile().NotNull();
          var solution = file.GetSolution();
          var statistics = solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>();
          statistics.TrackAction(T4StatisticIdBundle.PreprocessFromGutter);
          var preprocessingManager = solution.GetComponent<IT4TemplatePreprocessingManager>();
          var model = solution.GetProtocolSolution().GetT4ProtocolModel();
          model.PreprocessingStarted();
          model.PreprocessingFinished(preprocessingManager.Preprocess(file));
        }),
        new RichText(Strings.PreprocessTemplate_Text),
        null,
        BulbMenuAnchors.PermanentBackgroundItems
      );
    }

    [NotNull]
    private static ExecutableItem CreateRunFileExecutableItem(
      [NotNull] IT4TemplateDirective directive,
      [NotNull] Action<IT4TemplateExecutionManager, IT4File> execute,
      [NotNull] string statisticId
    ) => new ExecutableItem(() =>
    {
      var file = (IT4File)directive.GetContainingFile().NotNull();
      var solution = file.GetSolution();
      solution.GetComponent<Application.ActivityTrackingNew.UsageStatistics>().TrackAction(statisticId);
      var executionManager = solution.GetComponent<IT4TemplateExecutionManager>();
      executionManager.UpdateTemplateKind(file);
      execute(executionManager, file);
    });
  }
}