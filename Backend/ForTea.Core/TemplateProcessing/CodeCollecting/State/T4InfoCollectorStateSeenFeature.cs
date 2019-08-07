using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
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

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case T4FeatureBlock _: return this;
				default:
					Die();
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

		protected override bool FeatureStartedSafe => false;

		protected override void ConsumeTokenSafe(IT4Token token)
		{
			if (token.NodeType != T4TokenNodeTypes.NEW_LINE) LastToken = token;
		}

		protected override string ProduceSafe(ITreeNode lookahead) => null;
		protected override string ProduceBeforeEofSafe() => null;
	}
}
