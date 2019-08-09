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

		[NotNull]
		// Suspect for syntax error
		private ITreeNode FirstElement { get; }

		public T4InfoCollectorStateSeenFeatureAndText(
			[NotNull] StringBuilder builder,
			[NotNull] IT4CodeGenerationInterrupter interrupter,
			[NotNull] ITreeNode firstElement
		) : base(interrupter)
		{
			Builder = builder;
			FirstElement = firstElement;
		}

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
					var data = T4FailureRawData.FromElement(FirstElement, "Unexpected element after feature");
					Interrupter.InterruptAfterProblem(data);
					return this;
			}
		}

		protected override bool FeatureStartedSafe => true;
		protected override void ConsumeTokenSafe(IT4Token token) => Builder.Append(Convert(token));
		protected override string ProduceSafe(ITreeNode lookahead) => Builder.ToString();

		protected override string ProduceBeforeEofSafe()
		{
			var data = T4FailureRawData.FromElement(FirstElement, "Unexpected element after feature");
			Interrupter.InterruptAfterProblem(data);
			return Builder.ToString();
		}
	}
}
