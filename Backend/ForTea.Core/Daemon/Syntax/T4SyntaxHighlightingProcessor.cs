using System;
using GammaJul.ForTea.Core.Daemon.Attributes.GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Syntax
{
	public sealed class T4SyntaxHighlightingProcessor : SyntaxHighlightingProcessor
	{
		public override bool InteriorShouldBeProcessed(ITreeNode element, IHighlightingConsumer context)
		{
			var type = element.NodeType;
			if (type == ElementType.MACRO) return false;
			if (type == ElementType.ENVIRONMENT_VARIABLE) return false;
			return true;
		}

		public override void ProcessBeforeInterior(ITreeNode element, IHighlightingConsumer context)
		{
			var type = element.NodeType;
			if (type == ElementType.MACRO) HighlightMacro((IT4Macro) element, context);
			else if (type == ElementType.ENVIRONMENT_VARIABLE)
				HighlightEnvironmentVariable((IT4EnvironmentVariable) element, context);
			else if (type == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE) HighlightValue(element, context);
		}

		private static void HighlightMacro(
			[NotNull] IT4Macro element,
			[NotNull] IHighlightingConsumer context
		)
		{
			HighlightValue(element.Dollar, context);
			HighlightValue(element.LeftParenthesis, context);
			HighlightValue(element.RightParenthesis, context);
			var value = element.RawAttributeValue;
			if (value == null) return;
			var range = value.GetDocumentRange();
			context.AddHighlighting(new MacroHighlighting(range));
		}

		private static void HighlightEnvironmentVariable(
			[NotNull] IT4EnvironmentVariable element,
			[NotNull] IHighlightingConsumer context
		)
		{
			foreach (var node in element.StartPercent)
			{
				HighlightValue(node, context);
			}

			var value = element.RawAttributeValue;
			if (value == null) return;
			var range = value.GetDocumentRange();
			context.AddHighlighting(new EnvironmentVariableHighlighting(range));
		}

		private static void HighlightValue([CanBeNull] ITreeNode element, [NotNull] IHighlightingConsumer context)
		{
			if (element == null) return;
			const string id = T4HighlightingAttributeIds.RAW_ATTRIBUTE_VALUE;
			var highlighting = new ReSharperSyntaxHighlighting(id, null, element.GetDocumentRange());
			context.AddHighlighting(highlighting);
		}

		// These methods should never be called
		public override string GetAttributeId(TokenNodeType tokenType) => throw new NotSupportedException();
		protected override bool IsBlockComment(TokenNodeType tokenType) => throw new NotSupportedException();
		protected override bool IsLineComment(TokenNodeType tokenType) => throw new NotSupportedException();
		protected override bool IsString(TokenNodeType tokenType) => throw new NotSupportedException();
		protected override bool IsPreprocessor(TokenNodeType tokenType) => throw new NotSupportedException();
		protected override bool IsKeyword(TokenNodeType tokenType) => throw new NotSupportedException();
		protected override bool IsNumber(TokenNodeType tokenType) => throw new NotSupportedException();
		protected override string BlockCommentAttributeId => throw new NotSupportedException();
		protected override string LineCommentAttributeId => throw new NotSupportedException();
		protected override string StringAttributeId => throw new NotSupportedException();
		protected override string PreprocessorAttributeId => throw new NotSupportedException();
		protected override string KeywordAttributeId => throw new NotSupportedException();
		protected override string NumberAttributeId => throw new NotSupportedException();
	}
}
