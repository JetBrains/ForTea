using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public class T4InfoCollectorStateSeenFeatureAndText : T4InfoCollectorStateBase
	{
		[NotNull]
		private StringBuilder Builder { get; }

		public T4InfoCollectorStateSeenFeatureAndText(
			[NotNull] StringBuilder builder,
			[NotNull] IT4CodeGenerationInterrupter interrupter
		) : base(interrupter) => Builder = builder;

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case T4FeatureBlock _:
					Die();
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case T4ExpressionBlock _:
					Die();
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case IT4Token _: return this;
				default:
					Interrupter.InterruptAfterProblem();
					return this;
			}
		}

		protected override bool FeatureStartedSafe => true;
		protected override void ConsumeTokenSafe(IT4Token token) => Builder.Append(Convert(token));
		protected override string ProduceSafe(ITreeNode lookahead) => Builder.ToString();
		protected override string ProduceBeforeEofSafe()
		{
			Interrupter.InterruptAfterProblem();
			return Builder.ToString();
		}
	}
}
