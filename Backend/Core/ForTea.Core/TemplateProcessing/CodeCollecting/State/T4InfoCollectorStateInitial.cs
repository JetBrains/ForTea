using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
  public sealed class T4InfoCollectorStateInitial : T4InfoCollectorStateBase
  {
    [NotNull] private StringBuilder Builder { get; }

    public T4InfoCollectorStateInitial([NotNull] IT4CodeGenerationInterrupter interrupter) :
      this(new StringBuilder(), interrupter)
    {
    }

    public T4InfoCollectorStateInitial(
      [NotNull] StringBuilder builder,
      [NotNull] IT4CodeGenerationInterrupter interrupter
    ) : base(interrupter) => Builder = builder;

    public override IT4InfoCollectorState GetNextState(IT4TreeNode element)
    {
      switch (element)
      {
        case IT4FeatureBlock:
          return new T4InfoCollectorStateSeenFeature(Interrupter);
        case IT4IncludeDirective:
          return new T4InfoCollectorStateInitial(Interrupter);
        case IT4Directive:
        case IT4StatementBlock:
          return new T4InfoCollectorSateSeenSpecialBlock(Interrupter);
        case IT4ExpressionBlock:
          return new T4InfoCollectorStateInitial(Interrupter);
        default: return this;
      }
    }

    public override bool FeatureStarted => false;
    public override void ConsumeToken(IT4Token token) => Builder.Append(Convert(token));
    public override string Produce(ITreeNode lookahead) => Builder.ToString();
    public override string ProduceBeforeEof() => Builder.ToString();
  }
}