using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorStateSeenFeature : T4InfoCollectorStateBase
	{
		[CanBeNull]
		private IT4Token LastToken { get; set; }

		public T4InfoCollectorStateSeenFeature([NotNull] IT4CodeGenerationInterrupter interrupter) : base(interrupter)
		{
		}

		public override IT4InfoCollectorState GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4FeatureBlock _: return this;
				case IT4ExpressionBlock _:
					return new T4InfoCollectorStateSeenFeatureAndExpressionBlock(Interrupter);
				default:
					if (element.NodeType == T4TokenNodeTypes.NEW_LINE)
						return new T4InfoCollectorStateSeenFeatureAndNewLine(Interrupter);
					else if (element.NodeType == T4TokenNodeTypes.RAW_TEXT)
					{
						// At this point, LastToken is initialized through ConsumeTokenSafe call
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
