using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorSateSeenDirectiveOrStatementBlock : T4InfoCollectorStateBase
	{
		private StringBuilder Builder { get; }

		public T4InfoCollectorSateSeenDirectiveOrStatementBlock([NotNull] IT4CodeGenerationInterrupter interrupter) :
			base(interrupter) => Builder = new StringBuilder();

		public override IT4InfoCollectorState GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _:
				case IT4StatementBlock _: return this;
				case IT4FeatureBlock _:
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
