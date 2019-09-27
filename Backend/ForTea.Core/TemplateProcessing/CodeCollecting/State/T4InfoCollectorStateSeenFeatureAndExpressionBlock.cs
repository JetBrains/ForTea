using System;
using System.Text;
using GammaJul.ForTea.Core.Parsing;
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
	/// because there is an expression block after a feature block
	/// </summary>
	public class T4InfoCollectorStateSeenFeatureAndExpressionBlock : T4InfoCollectorStateBase
	{
		[CanBeNull]
		private IT4Token LastToken { get; set; }

		public T4InfoCollectorStateSeenFeatureAndExpressionBlock(
			[NotNull] IT4CodeGenerationInterrupter interrupter
		) : base(interrupter)
		{
		}

		public override IT4InfoCollectorState GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4FeatureBlock _:
					return new T4InfoCollectorStateSeenFeature(Interrupter);
				case IT4ExpressionBlock _:
					return this;
				default:
					if (element.NodeType == T4TokenNodeTypes.NEW_LINE)
					{
						var builder = new StringBuilder(Environment.NewLine);
						return new T4InfoCollectorStateSeenFeatureAndText(builder, Interrupter, element);
					}
					else if (element.NodeType == T4TokenNodeTypes.RAW_TEXT)
					{
						var builder = new StringBuilder(Convert(LastToken));
						return new T4InfoCollectorStateSeenFeatureAndText(builder, Interrupter, element);
					}

					var data = T4FailureRawData.FromElement(element, "Unexpected element after feature");
					Interrupter.InterruptAfterProblem(data);
					return this;
			}
		}

		public override bool FeatureStarted => true;

		public override void ConsumeToken(IT4Token token)
		{
			if (token.NodeType != T4TokenNodeTypes.NEW_LINE) LastToken = token;
		}

		public override string Produce(ITreeNode lookahead) => null;
		public override string ProduceBeforeEof() => null;
	}
}
