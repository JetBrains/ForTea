using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
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

		public override IT4InfoCollectorState GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4FeatureBlock _:
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case IT4ExpressionBlock _:
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case IT4Token _: return this;
				default:
					var data = T4FailureRawData.FromElement(FirstElement, "Unexpected element after feature");
					Interrupter.InterruptAfterProblem(data);
					return this;
			}
		}

		public override bool FeatureStarted => true;
		public override void ConsumeToken(IT4Token token) => Builder.Append(Convert(token));
		public override string Produce(ITreeNode lookahead) => Builder.ToString();

		public override string ProduceBeforeEof()
		{
			var data = T4FailureRawData.FromElement(FirstElement, "Unexpected element after feature");
			Interrupter.InterruptAfterProblem(data);
			return Builder.ToString();
		}
	}
}
