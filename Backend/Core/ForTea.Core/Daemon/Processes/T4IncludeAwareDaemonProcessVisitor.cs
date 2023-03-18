using System.Collections.Generic;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Utils;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
  // TODO: make it actually visitor
  public sealed class T4IncludeAwareDaemonProcessVisitor : IRecursiveElementProcessor
  {
    [NotNull] private T4IncludeGuard Guard { get; }

    [NotNull] private IT4IncludeResolver IncludeResolver { get; }

    [NotNull, ItemNotNull] private List<HighlightingInfo> MyHighlightings { get; } = new List<HighlightingInfo>();

    // Here I have the guarantee that new instance of this class is created for every analysis pass
    private bool SeenOutputDirective { get; set; }
    private bool SeenTemplateDirective { get; set; }

    [NotNull, ItemNotNull] public IReadOnlyList<HighlightingInfo> Highlightings => MyHighlightings;

    public T4IncludeAwareDaemonProcessVisitor([NotNull] IPsiSourceFile initialFile)
    {
      Guard = new T4IncludeGuard();
      Guard.StartProcessing(initialFile.GetLocation());
      IncludeResolver = initialFile.GetSolution().GetComponent<IT4IncludeResolver>();
    }

    public bool InteriorShouldBeProcessed(ITreeNode element) => true;

    public void ProcessAfterInterior(ITreeNode element)
    {
      if (element is IT4IncludedFile) Guard.EndProcessing();
    }

    public bool ProcessingIsFinished => false;

    public void ProcessBeforeInterior(ITreeNode element)
    {
      switch (element)
      {
        case IT4IncludeDirective include:
          ProcessInclude(include);
          break;
        case IT4IncludedFile include:
          Guard.StartProcessing(include.LogicalPsiSourceFile.GetLocation());
          break;
        case IT4Directive directive:
          ProcessDirective(directive);
          break;
      }
    }

    private void ProcessDirective(IT4Directive directive)
    {
      switch (directive)
      {
        case IT4OutputDirective when !SeenOutputDirective:
          SeenOutputDirective = true;
          break;
        case IT4OutputDirective:
          ReportDuplicateDirective(directive);
          break;
        case IT4TemplateDirective when !SeenTemplateDirective:
          SeenTemplateDirective = true;
          break;
        case IT4TemplateDirective:
          ReportDuplicateDirective(directive);
          break;
      }
    }

    private void ProcessInclude([NotNull] IT4IncludeDirective include)
    {
      var sourceFile = IncludeResolver.Resolve(include.ResolvedPath);
      if (sourceFile == null)
      {
        ReportUnresolvedPath(include);
        return;
      }

      if (!Guard.CanProcess(sourceFile.GetLocation())) return;
      if (include.Once && Guard.HasSeenFile(sourceFile.GetLocation()))
      {
        ReportRedundantInclude(include);
      }
    }

    private void ReportDuplicateDirective([NotNull] IT4Directive directive)
    {
      var warning = new IgnoredDirectiveWarning(directive);
      var highlightingInfo = new HighlightingInfo(directive.GetHighlightingRange(), warning);
      MyHighlightings.Add(highlightingInfo);
    }

    private void AddHighlighting([NotNull] ITreeNode node, [NotNull] IHighlighting highlighting) =>
      MyHighlightings.Add(new HighlightingInfo(node.GetHighlightingRange(), highlighting));

    private void ReportUnresolvedPath([NotNull] IT4IncludeDirective include)
    {
      var path = include.GetAttributeValueToken(T4DirectiveInfoManager.Include.FileAttribute.Name);
      if (path == null) return;
      AddHighlighting(path, new UnresolvedIncludeError(path));
    }

    private void ReportRedundantInclude([NotNull] IT4IncludeDirective include)
    {
      var value = include.GetFirstAttribute(T4DirectiveInfoManager.Include.FileAttribute)?.Value;
      if (value == null) return;
      AddHighlighting(value, new RedundantIncludeWarning(include));
    }
  }
}