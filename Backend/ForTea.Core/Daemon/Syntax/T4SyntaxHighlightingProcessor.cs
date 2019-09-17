using System;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Syntax
{
	public sealed class T4SyntaxHighlightingProcessor : SyntaxHighlightingProcessor
	{
		private T4SyntaxHighlightingVisitor Visitor { get; } = new T4SyntaxHighlightingVisitor();

		public override bool InteriorShouldBeProcessed(ITreeNode element, IHighlightingConsumer context)
		{
			var type = element.NodeType;
			if (type == ElementType.MACRO) return false;
			if (type == ElementType.ENVIRONMENT_VARIABLE) return false;
			return true;
		}

		public override void ProcessBeforeInterior(ITreeNode element, IHighlightingConsumer context)
		{
			if (!(element is IT4TreeNode t4Element)) return;
			t4Element.Accept(Visitor, context);
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
