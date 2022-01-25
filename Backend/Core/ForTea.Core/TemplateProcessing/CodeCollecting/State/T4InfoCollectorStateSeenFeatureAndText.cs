using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	/// <summary>
	/// If a feature block is followed by newlines, they get ignored,
	/// see <see cref="T4InfoCollectorStateSeenFeatureAndNewLine"/>.
	/// This state represents that newlines should not be ignored anymore
	/// because there is text after a feature block
	/// </summary>
	public class T4InfoCollectorStateSeenFeatureAndText : T4InfoCollectorStateBase
	{
		[NotNull]
		private StringBuilder Builder { get; }

		[NotNull]
		// Suspect for syntax error
		private IT4TreeNode FirstElement { get; }

		[NotNull]
		public override IT4TreeNode FirstNode => FirstElement;

		public T4InfoCollectorStateSeenFeatureAndText(
			[NotNull] StringBuilder builder,
			[NotNull] IT4CodeGenerationInterrupter interrupter,
			[NotNull] IT4TreeNode firstElement
		) : base(interrupter)
		{
			Builder = builder;
			FirstElement = firstElement;
		}

		public override IT4InfoCollectorState GetNextState(IT4TreeNode element)
		{
			switch (element)
			{
				case IT4FeatureBlock:
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case IT4ExpressionBlock:
					return new T4InfoCollectorStateSeenFeatureAndExpressionBlock(Interrupter);
				case IT4Token: return this;
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
