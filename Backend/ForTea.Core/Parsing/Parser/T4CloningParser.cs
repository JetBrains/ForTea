using System;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Ranges;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing.Parser
{
	public sealed class T4CloningParser : IParser
	{
		[NotNull]
		private IPsiSourceFile RootSourceFile { get; }

		[NotNull]
		private T4RangeTranslatorInitializer RangeTranslatorInitializer { get; }

		[NotNull]
		private T4CloningParserVisitor CloningParserVisitor { get; }

		public T4CloningParser(
			[NotNull] IPsiSourceFile rootSourceFile,
			[NotNull] IPsiSourceFile sourceFile,
			[NotNull] IT4LexerSelector selector
		)
		{
			RootSourceFile = rootSourceFile;
			RangeTranslatorInitializer = new T4RangeTranslatorInitializer();
			CloningParserVisitor = new T4CloningParserVisitor(selector, sourceFile);
		}

		[NotNull]
		public IFile ParseFile()
		{
			var primaryPsiFile = RootSourceFile.GetPrimaryPsiFile();
			if (primaryPsiFile is not IT4File t4File)
			{
				throw new InvalidOperationException("The root file has to be a T4 file");
			}

			var clone = (File) CloneDescendents(t4File);
			RangeTranslatorInitializer.SetUpRangeTranslators(clone);
			return clone;
		}

		[NotNull]
		private IT4TreeNode CloneDescendents([NotNull] IT4TreeNode node)
		{
			var result = node.Accept(CloningParserVisitor, Unit.Instance);
			var clonedNode = result.CurrentClone.NotNull();
			if (!result.ShouldContinueRecursiveDescent) return clonedNode;
			foreach (var child in node.Children())
			{
				var clonedChild = CloneDescendents((IT4TreeNode) child);
				((CompositeElement) clonedNode).AppendNewChild((TreeElement) clonedChild);
			}

			return clonedNode;
		}
	}
}
