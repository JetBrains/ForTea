using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp.Impl.Tree;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Web.CodeBehindSupport;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	[ShellComponent]
	public class T4CSharpCustomIndentHandler : ICustomIndentHandler
	{
		public string Indent(
			ITreeNode node,
			CustomIndentType indentType,
			FmtSettings<CSharpFormatSettingsKey> settings
		)
		{
			var file = node.GetContainingNode<IFile>(true);
			if (!IsInT4File(file)) return null;
			if (indentType != CustomIndentType.DirectCalculation) return null;
			if (!node.IsPhysical()) return null;
			if (node.GetT4ContainerFromCSharpNode<IT4CodeBlock>() == null) return "";

			if (node is ITokenNode tokenNode
			    && tokenNode.GetTokenType().IsComment
			    && tokenNode.GetText() == T4CodeBehindFormatProvider.Instance.CodeCommentEnd)
				return HandleComment(file, tokenNode);
			var rangeTranslator = file.GetRangeTranslator();
			var originalFile = rangeTranslator.OriginalFile;

			var statement = node as ICSharpStatement;
			if (node.GetTokenType() == CSharpTokenType.LBRACE)
				statement = node.Parent as IBlock;
			var block = BlockNavigator.GetByStatement(statement);
			if (node is IComment || node is IStartRegion || node is IEndRegion)
				block = node.GetContainingNode<ITreeNode>() as IBlock;
			if (block == null)
				return "";

			var generatedTreeRange = new TreeTextRange(node.GetTreeStartOffset());
			var blockGeneratedTreeRange = new TreeTextRange(block.GetTreeStartOffset());
			var originalRange = rangeTranslator.GeneratedToOriginal(generatedTreeRange);
			var blockOriginalRange = rangeTranslator.GeneratedToOriginal(blockGeneratedTreeRange);
			if (!originalRange.IsValid())
				return null;

			//var originalFileText = originalFile.GetText();
			var aspElement = originalFile.FindNodeAt(originalRange);

			if (!(aspElement?.Parent?.Parent is T4CodeBlock codeBlock)) return null;
			// find previous statement
			for (var nd = node.PrevSibling; nd != null; nd = nd.PrevSibling)
			{
				if (!(nd is IStatement))
					continue;
				var token = nd.GetFirstTokenIn();
				var tmpRange = token.GetDocumentRange();
				if (!tmpRange.IsValid() || tmpRange.TextRange.EndOffset >= originalFile.GetTextLength())
					continue;

				var generatedTreeRange1 = new TreeTextRange(token.GetTreeStartOffset());
				var originalRange1 = rangeTranslator.GeneratedToOriginal(generatedTreeRange1);
				var aspElement1 = originalFile.FindNodeAt(originalRange1);
				var codeBlock1 = aspElement1?.Parent as AspRenderBlock;
				if (codeBlock1 == null || codeBlock.Parent != codeBlock1.Parent)
					break;

				return tmpRange.GetIndentFromDocumentRange();
			}

			if (blockOriginalRange.IsValid() && codeBlock.GetTreeTextRange().Contains(blockOriginalRange))
				return null;
			var blockStart = codeBlock.GetTreeStartOffset();
			var nodeStart = originalRange.StartOffset.Offset;
			var hasLineBreak =
				codeBlock.GetText().Substring(0, nodeStart - blockStart.Offset).IndexOf('\n') >= 0;
			if (hasLineBreak)
				return "";

			return null;
		}

		[ContractAnnotation("null => false")]
		private static bool IsInT4File([CanBeNull] IFile file)
		{
			var sourceFile = file?.GetSourceFile();
			return sourceFile?.LanguageType.Is<T4ProjectFileType>() == true;
		}

		private static string HandleComment(IFile file, ITokenNode tokenNode)
		{
			var rangeTranslator = file.GetRangeTranslator();
			var startOffset = tokenNode.GetTreeStartOffset();
			var generatedTreeRange = new TreeTextRange(startOffset - 1, startOffset);
			var originalTreeRange = rangeTranslator.GeneratedToOriginal(generatedTreeRange);
			if (!originalTreeRange.IsValid()) return null;
			var t4File = rangeTranslator.OriginalFile;
			var t4Element = t4File.FindNodeAt(originalTreeRange);
			var block = t4Element?.GetContainingNode<ITreeNode>(n => n is IT4CodeBlock, true);
			return block?.GetIndentViaDocument();
		}
	}
}
