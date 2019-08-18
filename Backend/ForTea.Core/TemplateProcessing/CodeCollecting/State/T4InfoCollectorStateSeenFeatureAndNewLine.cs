using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public class T4InfoCollectorStateSeenFeatureAndNewLine : T4InfoCollectorStateBase
	{
		[NotNull]
		private StringBuilder Builder { get; }

		public T4InfoCollectorStateSeenFeatureAndNewLine([NotNull] IT4CodeGenerationInterrupter interrupter) :
			base(interrupter) => Builder = new StringBuilder();

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case T4FeatureBlock _:
					Die();
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case T4ExpressionBlock _:
					Die();
					return new T4InfoCollectorStateInitial(Interrupter);
				default:
					if (element.NodeType == T4TokenNodeTypes.NEW_LINE) return this;
					else if (element.NodeType == T4TokenNodeTypes.RAW_TEXT)
					{
						Die();
						return new T4InfoCollectorStateSeenFeatureAndText(Builder, Interrupter, element);
					}

					var data = T4FailureRawData.FromElement(element, "Unexpected element after feature");
					Interrupter.InterruptAfterProblem(data);
					return this;
			}
		}

		protected override bool FeatureStartedSafe => true;
		protected override void ConsumeTokenSafe(IT4Token token) => Builder.Append(Convert(token));
		protected override string ProduceBeforeEofSafe() => null;

		protected override string ProduceSafe(ITreeNode lookahead)
		{
			if (lookahead is T4FeatureBlock) return null;
			return Builder.ToString();
		}
	}
}
