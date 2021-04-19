using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser.Impl;
using GammaJul.ForTea.Core.Parsing.Ranges;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing.Parser
{
	public sealed class T4CloningParser : IParser
	{
		[NotNull]
		private IT4PsiFileProvider PsiFileProvider { get; }

		[NotNull]
		private T4RangeTranslatorInitializer RangeTranslatorInitializer { get; }

		[NotNull]
		private T4CloningParserVisitor CloningParserVisitor { get; }

		public T4CloningParser(
			[NotNull] IPsiSourceFile rootSourceFile,
			[CanBeNull] IPsiSourceFile physicalSourceFile,
			[NotNull] IT4LexerSelector selector
		) : this(new T4PsiFromSourceProvider(rootSourceFile), physicalSourceFile, selector)
		{
		}

		public T4CloningParser(
			[NotNull] IT4PsiFileProvider psiFileProvider,
			[CanBeNull] IPsiSourceFile physicalSourceFile,
			[NotNull] IT4LexerSelector selector
		)
		{
			PsiFileProvider = psiFileProvider;
			RangeTranslatorInitializer = new T4RangeTranslatorInitializer();
			CloningParserVisitor = new T4CloningParserVisitor(selector, physicalSourceFile);
		}

		[NotNull]
		public IFile ParseFile()
		{
			var t4File = PsiFileProvider.GetPsi();
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
