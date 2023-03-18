using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
  // Some blocks followed by a newline cause that newline to be ignored.
  // These block are:
  // - all directive blocks except <#@ include #>
  // - all statement blocks
  // Note that feature blocks also cause newlines to be ignored
  // but their logic is more complex and is implemented in a separate state
  public sealed class T4InfoCollectorSateSeenSpecialBlock : T4InfoCollectorStateBase
  {
    private StringBuilder Builder { get; }

    public T4InfoCollectorSateSeenSpecialBlock([NotNull] IT4CodeGenerationInterrupter interrupter) :
      base(interrupter) => Builder = new StringBuilder();

    public override IT4InfoCollectorState GetNextState(IT4TreeNode element)
    {
      switch (element)
      {
        case IT4IncludeDirective: return new T4InfoCollectorStateInitial(Builder, Interrupter);
        case IT4Directive:
        case IT4StatementBlock: return this;
        case IT4FeatureBlock:
          return new T4InfoCollectorStateSeenFeature(Interrupter);
        default:
          return new T4InfoCollectorStateInitial(Builder, Interrupter);
      }
    }

    public override bool FeatureStarted => false;

    public override void ConsumeToken(IT4Token token)
    {
      if (token.NodeType != T4TokenNodeTypes.NEW_LINE) Builder.Append(Convert(token));
    }

    public override string Produce(ITreeNode lookahead) => null;
    public override string ProduceBeforeEof() => null;
  }
}