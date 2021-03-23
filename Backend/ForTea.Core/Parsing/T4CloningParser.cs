using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using AttributeValue = GammaJul.ForTea.Core.Tree.Impl.AttributeValue;

namespace GammaJul.ForTea.Core.Parsing
{
	public sealed class T4CloningParser : IParser
	{
		[NotNull]
		private IPsiSourceFile RootSourceFile { get; }

		[NotNull]
		private IPsiSourceFile SourceFile { get; }

		[NotNull]
		private IT4LexerSelector Selector { get; }

		public T4CloningParser(
			[NotNull] IPsiSourceFile rootSourceFile,
			[NotNull] IPsiSourceFile sourceFile,
			[NotNull] IT4LexerSelector selector
		)
		{
			RootSourceFile = rootSourceFile;
			SourceFile = sourceFile;
			Selector = selector;
			var x = new KeyValuePair<int, string>();
			var y = x.Value;
		}

		public IFile ParseFile()
		{
			var primaryPsiFile = RootSourceFile.GetPrimaryPsiFile();
			if (primaryPsiFile is not IT4File t4File)
			{
				throw new InvalidOperationException("The root file has to be a T4 file");
			}

			return (IFile) CloneDescendents(t4File);
		}

		private static IT4TreeNode CloneDescendents([NotNull] IT4File file)
		{
			var stackOfHandledItems = new Stack<IT4TreeNode>();
			bool currentStackTopChildrenAreHandled = false;
			var clones = new Stack<IT4TreeNode>();
			var visitor = new CloningVisitor();
			file.Accept(visitor);
			var root = visitor.CurrentClone;
			clones.Push(root);
			stackOfHandledItems.Push(file);
			do
			{
				if (currentStackTopChildrenAreHandled)
				{
					var top = stackOfHandledItems.Pop();
					clones.Pop();
					var next = (IT4TreeNode) top.NextSibling;
					if (next == null) continue;
					next.Accept(visitor);
					((T4CompositeElement) clones.Peek()).AppendNewChild((TreeElement) visitor.CurrentClone);
					clones.Push(visitor.CurrentClone);
					stackOfHandledItems.Push(next);
					currentStackTopChildrenAreHandled = false;
				}

				var lastHandledItem = stackOfHandledItems.Peek();
				var unhandledChildren = lastHandledItem.Children().AsList();
				if (unhandledChildren.Count == 0)
				{
					currentStackTopChildrenAreHandled = true;
					continue;
				}

				var child = (IT4TreeNode) unhandledChildren.First();
				child.Accept(visitor);
				((CompositeElement) clones.Peek()).AppendNewChild((TreeElement) visitor.CurrentClone);
				clones.Push(visitor.CurrentClone);
				stackOfHandledItems.Push(child);
			}
			while (!stackOfHandledItems.IsEmpty());

			return root;
		}

		private sealed class CloningVisitor : TreeNodeVisitor
		{
			[CanBeNull]
			public IT4TreeNode CurrentClone { get; private set; }

			public override void VisitNode(ITreeNode node)
			{
				if (node.NodeType is not TokenNodeType type)
				{
					CurrentClone = null;
					return;
				}

				CurrentClone = (IT4TreeNode) type.Create(node.GetText());
			}

			public override void VisitAssemblyDirectiveNode(IT4AssemblyDirective assemblyDirectiveParam) => CurrentClone = new AssemblyDirective();
			public override void VisitAttributeNameNode(IT4AttributeName attributeNameParam) => CurrentClone = new AttributeName();
			public override void VisitAttributeValueNode(IT4AttributeValue attributeValueParam) => CurrentClone = new AttributeValue();
			public override void VisitBlockNode(IT4Block blockParam) => throw new InvalidOperationException();
			public override void VisitCleanupBehaviorDirectiveNode(IT4CleanupBehaviorDirective cleanupBehaviorDirectiveParam) => CurrentClone = new CleanupBehaviorDirective();
			public override void VisitCodeNode(IT4Code codeParam) => CurrentClone = new Code();
			public override void VisitCodeBlockNode(IT4CodeBlock codeBlockParam) => throw new InvalidOperationException();
			public override void VisitDirectiveNode(IT4Directive directiveParam) => throw new InvalidOperationException();
			public override void VisitDirectiveAttributeNode(IT4DirectiveAttribute directiveAttributeParam) => CurrentClone = new DirectiveAttribute();
			public override void VisitEnvironmentVariableNode(IT4EnvironmentVariable environmentVariableParam) => CurrentClone = new EnvironmentVariable();
			public override void VisitExpressionBlockNode(IT4ExpressionBlock expressionBlockParam) => CurrentClone = new ExpressionBlock();
			public override void VisitFeatureBlockNode(IT4FeatureBlock featureBlockParam) => CurrentClone = new FeatureBlock();
			public override void VisitFileNode(IT4File fileParam) => CurrentClone = new File();
			public override void VisitImportDirectiveNode(IT4ImportDirective importDirectiveParam) => CurrentClone = new ImportDirective();
			public override void VisitIncludeDirectiveNode(IT4IncludeDirective includeDirectiveParam) => CurrentClone = new IncludeDirective();

			public override void VisitIncludedFileNode(IT4IncludedFile includedFileParam)
			{
				CurrentClone = new IncludedFile();
			}

			public override void VisitMacroNode(IT4Macro macroParam) => CurrentClone = new Macro();
			public override void VisitOutputDirectiveNode(IT4OutputDirective outputDirectiveParam) => CurrentClone = new OutputDirective();
			public override void VisitParameterDirectiveNode(IT4ParameterDirective parameterDirectiveParam) => CurrentClone = new ParameterDirective();
			public override void VisitStatementBlockNode(IT4StatementBlock statementBlockParam) => CurrentClone = new StatementBlock();
			public override void VisitTemplateDirectiveNode(IT4TemplateDirective templateDirectiveParam) => CurrentClone = new TemplateDirective();
			public override void VisitUnknownDirectiveNode(IT4UnknownDirective unknownDirectiveParam) => CurrentClone = new UnknownDirective();
		}
	}
}
