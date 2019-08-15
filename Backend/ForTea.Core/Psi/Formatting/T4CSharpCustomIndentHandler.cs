using System;
using System.Linq;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Impl.Shared;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Web.CodeBehindSupport;
using JetBrains.RiderTutorials.Utils;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	[ShellComponent]
	public class T4CSharpCustomIndentHandler : ICustomIndentHandler
	{
		[CanBeNull]
		public string Indent(
			[NotNull] ITreeNode node,
			CustomIndentType indentType,
			[NotNull] FmtSettings<CSharpFormatSettingsKey> settings
		)
		{
			if (node == null) throw new ArgumentNullException(nameof(node));
			if (settings == null) throw new ArgumentNullException(nameof(settings));
			if (!node.IsPhysical()) return null;
			if (!IsInT4File(node)) return null;
			if (indentType == CustomIndentType.RelativeNodeCalculation) return node.GetIndentViaDocument();
			if (IsEndComment(node)) return IndentBlockEnd();
			if (IsTransformTextMember(node)) return IndentTransformTextMember(node, indentType);
			if (IsFeatureBlockMember(node)) return IndentFeatureBlockMember(node);
			return null;
		}

		[Pure]
		[CanBeNull]
		private string IndentFeatureBlockMember([NotNull] ITreeNode node)
		{
			if (!IsTypeMemberLikeNode(node)) return null;
			return "    ";
		}

		[Pure]
		[NotNull]
		private static string IndentBlockEnd() => "";

		[Pure]
		[CanBeNull]
		private string IndentTransformTextMember(
			[NotNull] ITreeNode node,
			CustomIndentType indentType
		)
		{
			var rangeTranslator = GetRangeTranslator(node);

			if (indentType == CustomIndentType.RelativeLineCalculation)
				return CalculateRelativeIndentInTransformText(node, rangeTranslator);

			var statement = node as ICSharpStatement;
			if (node.GetTokenType() == CSharpTokenType.LBRACE)
				statement = node.Parent as IBlock;
			var block = BlockNavigator.GetByStatement(statement);
			if (node is IComment || node is IStartRegion || node is IEndRegion)
				block = node.GetContainingNode<ITreeNode>() as IBlock;
			if (block == null) return null;

			var generatedTreeRange = new TreeTextRange(node.GetTreeStartOffset());
			var blockGeneratedTreeRange = new TreeTextRange(block.GetTreeStartOffset());
			var originalRange = rangeTranslator.GeneratedToOriginal(generatedTreeRange);
			var blockOriginalRange = rangeTranslator.GeneratedToOriginal(blockGeneratedTreeRange);
			if (!originalRange.IsValid()) return null;

			var t4Element = rangeTranslator.OriginalFile.FindNodeAt(originalRange);
			var codeBlock = t4Element?.GetParentOfType<IT4CodeBlock>();
			if (codeBlock == null) return null;

			string indentFromPreviousStatement = GetIndentFromPreviousStatement(node, rangeTranslator);
			if (indentFromPreviousStatement != null) return indentFromPreviousStatement;

			if (blockOriginalRange.IsValid() && codeBlock.GetTreeTextRange().Contains(blockOriginalRange))
				return null;
			var blockStart = codeBlock.GetTreeStartOffset();
			int nodeStart = originalRange.StartOffset.Offset;
			if (!HasLineBreak(codeBlock, nodeStart, blockStart)) return null;
			if (HasVisibleTokenBefore(rangeTranslator, node)) return null;
			return "    "; // TODO: use settings
		}

		private static RangeTranslatorWithGeneratedRangeMap GetRangeTranslator(ITreeNode node) =>
			node.GetContainingNode<IFile>(true).NotNull().GetRangeTranslator();

		private static string CalculateRelativeIndentInTransformText(
			[NotNull] ITreeNode node,
			[NotNull] ISecondaryRangeTranslator rangeTranslator
		)
		{
			var firstToken = node.GetFirstTokenIn();
			var generatedTreeRange1 = new TreeTextRange(firstToken.GetTreeStartOffset());
			var originalRange = rangeTranslator.GeneratedToOriginal(generatedTreeRange1);
			if (!originalRange.IsValid()) return null;
			var t4Element = rangeTranslator.OriginalFile.FindNodeAt(originalRange);
			return t4Element?.GetLineIndentFromOriginalNode(n =>
				n is IT4Token token && token.GetTokenType() == T4TokenNodeTypes.RAW_CODE, originalRange.StartOffset);
		}

		[Pure]
		private static bool HasVisibleTokenBefore(
			[NotNull] ISecondaryRangeTranslator rangeTranslator,
			[NotNull] ITreeNode node
		) => node.GetFirstTokenIn()
			.PrevTokens()
			.Where(token => !token.IsWhitespaceToken())
			.Select(token => token.GetTreeTextRange())
			.Select(rangeTranslator.GeneratedToOriginal)
			.Where(originalRange => originalRange.IsValid())
			.SelectNotNull(rangeTranslator.OriginalFile.FindNodeAt)
			.SelectNotNull(it => it.GetParentOfType<IT4CodeBlock>())
			.Any();

		[Pure]
		private static bool HasLineBreak([NotNull] IT4CodeBlock codeBlock, int nodeStart, TreeOffset blockStart) =>
			codeBlock.GetText().Substring(0, nodeStart - blockStart.Offset).IndexOf('\n') >= 0;

		[Pure]
		private static string GetIndentFromPreviousStatement(
			[NotNull] ITreeNode node,
			[NotNull] RangeTranslatorWithGeneratedRangeMap rangeTranslator
		)
		{
			for (var currentNode = node.PrevSibling; currentNode != null; currentNode = currentNode.PrevSibling)
			{
				string indent = TryGetIndentFromStatement(rangeTranslator, currentNode);
				if (indent != null) return indent;
			}

			return null;
		}

		[Pure]
		[CanBeNull]
		private static string TryGetIndentFromStatement(
			[NotNull] ISecondaryRangeTranslator rangeTranslator,
			[NotNull] ITreeNode currentNode
		)
		{
			if (!(currentNode is IStatement)) return null;
			var token = currentNode.GetFirstTokenIn();
			var tokenRange = token.GetDocumentRange();
			if (!tokenRange.IsValid()) return null;
			if (tokenRange.TextRange.EndOffset >= rangeTranslator.OriginalFile.GetTextLength()) return null;
			var generatedTokenRange = new TreeTextRange(token.GetTreeStartOffset());
			var originalTokenRange = rangeTranslator.GeneratedToOriginal(generatedTokenRange);
			var t4Element = rangeTranslator.OriginalFile.FindNodeAt(originalTokenRange);
			var otherCodeBlock = t4Element?.GetParentOfType<T4CodeBlock>();
			if (otherCodeBlock == null) return null;
			return tokenRange.GetIndentFromDocumentRange();
		}

		[Pure]
		private static bool IsTransformTextMember([NotNull] ITreeNode node)
		{
			var block = node.GetFirstTokenIn().GetT4ContainerFromCSharpNode<IT4CodeBlock>();
			return block != null && !(block is T4FeatureBlock);
		}

		[Pure]
		private static bool IsFeatureBlockMember([NotNull] ITreeNode node) =>
			node.GetFirstTokenIn().GetT4ContainerFromCSharpNode<T4FeatureBlock>() != null;

		[Pure]
		private static bool IsEndComment([NotNull] ITreeNode node) =>
			node is ITokenNode tokenNode
			&& tokenNode.GetTokenType().IsComment
			&& tokenNode.GetText() == T4CSharpCodeBehindIntermediateConverter.CodeCommentEndText;

		[Pure]
		private static bool IsTypeMemberLikeNode([NotNull] ITreeNode node) =>
			(node is ITypeMemberDeclaration || node is IMultipleFieldDeclaration || node is IMultipleEventDeclaration)
			&& !(node is IEventDeclaration)
			&& node.GetContainingNode<IClassLikeDeclaration>() != null;

		[Pure]
		[ContractAnnotation("null => false")]
		private static bool IsInT4File([CanBeNull] ITreeNode file) =>
			file?.GetSourceFile()?.LanguageType.Is<T4ProjectFileType>() == true;
	}
}
