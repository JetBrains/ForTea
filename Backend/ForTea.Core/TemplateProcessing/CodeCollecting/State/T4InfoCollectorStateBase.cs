using System;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public abstract class T4InfoCollectorStateBase : IT4InfoCollectorState
	{
		[NotNull]
		protected IT4CodeGenerationInterrupter Interrupter { get; }

		// This property is only required for ensuring correctness of state management code
		protected T4InfoCollectorStateBase([NotNull] IT4CodeGenerationInterrupter interrupter) =>
			Interrupter = interrupter;

		public abstract string Produce(ITreeNode lookahead);
		public abstract string ProduceBeforeEof();
		public abstract IT4InfoCollectorState GetNextState(ITreeNode element);
		public abstract bool FeatureStarted { get; }
		public abstract void ConsumeToken(IT4Token token);

		[NotNull]
		protected static string Convert([CanBeNull] IT4Token token) => StringLiteralConverter.EscapeToRegular(
			token?.NodeType == T4TokenNodeTypes.NEW_LINE
				? Environment.NewLine // todo: use \n and change it to environmental newline later
				: token?.GetText());
	}
}
