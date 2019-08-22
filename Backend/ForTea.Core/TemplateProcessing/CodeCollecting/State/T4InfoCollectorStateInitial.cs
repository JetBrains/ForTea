using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorStateInitial : T4InfoCollectorStateBase
	{
		[NotNull]
		private StringBuilder Builder { get; }

		public T4InfoCollectorStateInitial([NotNull] IT4CodeGenerationInterrupter interrupter) :
			this(new StringBuilder(), interrupter)
		{
		}

		public T4InfoCollectorStateInitial(
			[NotNull] StringBuilder builder,
			[NotNull] IT4CodeGenerationInterrupter interrupter
		) : base(interrupter) => Builder = builder;

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case IT4FeatureBlock _:
					Die();
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case IT4Directive _:
				case IT4StatementBlock _:
					Die();
					return new T4InfoCollectorSateSeenDirectiveOrStatementBlock(Interrupter);
				case IT4ExpressionBlock _:
					Die();
					return new T4InfoCollectorStateInitial(Interrupter);
				default: return this;
			}
		}

		protected override bool FeatureStartedSafe => false;
		protected override void ConsumeTokenSafe(IT4Token token) => Builder.Append(Convert(token));
		protected override string ProduceSafe(ITreeNode lookahead) => Builder.ToString();
		protected override string ProduceBeforeEofSafe() => Builder.ToString();
	}
}
