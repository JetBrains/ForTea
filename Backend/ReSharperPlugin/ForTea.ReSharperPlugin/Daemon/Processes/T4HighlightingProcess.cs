using System;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Attributes;
using GammaJul.ForTea.Core.Daemon.Syntax;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Processes
{
  /// <summary>Process that highlights block tags and missing token errors.</summary>
  internal sealed class T4HighlightingProcess : IDaemonStageProcess, IRecursiveElementProcessor<IHighlightingConsumer>
  {
    public IDaemonProcess DaemonProcess { get; }

    /// <summary>Gets the associated T4 file.</summary>
    private IT4File File { get; }

    public bool InteriorShouldBeProcessed(ITreeNode element, IHighlightingConsumer consumer) =>
      !(element is IT4File);

    public void ProcessAfterInterior(ITreeNode element, IHighlightingConsumer consumer)
    {
    }

    public bool IsProcessingFinished(IHighlightingConsumer consumer) => false;

    public void Execute(Action<DaemonStageResult> commiter)
    {
      var sourceFile = File.PhysicalPsiSourceFile;
      if (sourceFile == null) return;
      var consumer = new DefaultHighlightingConsumer(sourceFile);
      File.ProcessDescendants(this, consumer);
      var solution = File.GetSolution();
      var relevantHighlightings = consumer
        .Highlightings
        .Where(info => info.Range.Document.GetPsiSourceFile(solution) == sourceFile);
      commiter(new DaemonStageResult(relevantHighlightings.ToArray()));
    }

    public void ProcessBeforeInterior(ITreeNode element, IHighlightingConsumer consumer)
    {
      string attributeId = GetHighlightingAttributeId(element);
      if (attributeId != null)
      {
        DocumentRange range = element.GetHighlightingRange();
        consumer.AddHighlighting(new ReSharperSyntaxHighlighting(attributeId, string.Empty, range));
      }

      if (!(element is IT4TreeNode t4Element)) return;
      var visitor = new T4SyntaxHighlightingVisitor(consumer);
      t4Element.Accept(visitor);
    }

    [CanBeNull]
    private static string GetHighlightingAttributeId([NotNull] ITreeNode element)
    {
      if (!(element.GetTokenType() is T4TokenNodeType tokenType)) return null;
      if (tokenType.IsTag) return T4HighlightingAttributeIds.BLOCK_TAG;
      if (tokenType == T4TokenNodeTypes.QUOTE
          || tokenType == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE
          || tokenType == T4TokenNodeTypes.EQUAL
          || tokenType == T4TokenNodeTypes.DOLLAR
          || tokenType == T4TokenNodeTypes.PERCENT
          || tokenType == T4TokenNodeTypes.LEFT_PARENTHESIS
          || tokenType == T4TokenNodeTypes.RIGHT_PARENTHESIS)
        return T4HighlightingAttributeIds.ATTRIBUTE_VALUE;
      if (T4Lexer.DirectiveTypes[tokenType]) return T4HighlightingAttributeIds.DIRECTIVE;
      if (tokenType == T4TokenNodeTypes.TOKEN) return T4HighlightingAttributeIds.DIRECTIVE_ATTRIBUTE;

      return null;
    }

    internal T4HighlightingProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess)
    {
      File = file;
      DaemonProcess = daemonProcess;
    }
  }
}