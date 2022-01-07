using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	/// <summary>
	/// All newlines that appear after a feature block should be ignored,
	/// no matter how many of them there are.
	/// After any non-newline token, however,
	/// newlines should once again be taken into account
	/// </summary>
	public class T4InfoCollectorStateSeenFeatureAndNewLine : T4InfoCollectorStateBase
	{
		[NotNull]
		private StringBuilder Builder { get; }

		public T4InfoCollectorStateSeenFeatureAndNewLine([NotNull] IT4CodeGenerationInterrupter interrupter) :
			base(interrupter) => Builder = new StringBuilder();

		public override IT4InfoCollectorState GetNextState(IT4TreeNode element)
		{
			switch (element)
			{
				case IT4FeatureBlock:
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case IT4ExpressionBlock:
					return new T4InfoCollectorStateSeenFeatureAndExpressionBlock(Interrupter);
				default:
					if (element.NodeType == T4TokenNodeTypes.NEW_LINE) return this;
					else if (element.NodeType == T4TokenNodeTypes.RAW_TEXT)
					{
						return new T4InfoCollectorStateSeenFeatureAndText(Builder, Interrupter, element);
					}

					var data = T4FailureRawData.FromElement(element, "Unexpected element after feature");
					Interrupter.InterruptAfterProblem(data);
					return this;
			}
		}

		public override bool FeatureStarted => true;
		public override void ConsumeToken(IT4Token token) => Builder.Append(Convert(token));
		public override string ProduceBeforeEof() => null;

		public override string Produce(ITreeNode lookahead)
		{
			if (lookahead is IT4FeatureBlock) return null;
			return Builder.ToString();
		}
	}
}
