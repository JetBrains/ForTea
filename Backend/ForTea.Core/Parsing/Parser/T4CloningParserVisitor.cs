using System;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using AttributeValue = GammaJul.ForTea.Core.Tree.Impl.AttributeValue;

namespace GammaJul.ForTea.Core.Parsing.Parser
{
	internal sealed class T4CloningParserVisitor : TreeNodeVisitor<Unit, T4NodeCloningResult>
	{
		[NotNull]
		private IT4LexerSelector LexerSelector { get; }

		[NotNull]
		private IPsiSourceFile PhysicalSourceFile { get; }

		public T4CloningParserVisitor(
			[NotNull] IT4LexerSelector lexerSelector,
			[NotNull] IPsiSourceFile physicalSourceFile
		)
		{
			LexerSelector = lexerSelector;
			PhysicalSourceFile = physicalSourceFile;
		}

		private T4NodeCloningResult CreateResult(IT4TreeNode result) => new(true, result);

		public override T4NodeCloningResult VisitNode(ITreeNode node, Unit context)
		{
			if (node.NodeType is not TokenNodeType type) throw new InvalidOperationException();
			return CreateResult((IT4TreeNode) type.Create(node.GetText()));
		}

		public override T4NodeCloningResult VisitAssemblyDirectiveNode(IT4AssemblyDirective assemblyDirectiveParam, [CanBeNull] Unit context)
		{
			var result = new AssemblyDirective();
			result.InitializeResolvedPath(assemblyDirectiveParam.ResolvedPath);
			return CreateResult(result);
		}

		public override T4NodeCloningResult VisitAttributeNameNode(IT4AttributeName attributeNameParam, [CanBeNull] Unit context) => CreateResult(new AttributeName());
		public override T4NodeCloningResult VisitAttributeValueNode(IT4AttributeValue attributeValueParam, [CanBeNull] Unit context) => CreateResult(new AttributeValue());
		public override T4NodeCloningResult VisitBlockNode(IT4Block blockParam, [CanBeNull] Unit context) => throw new InvalidOperationException();
		public override T4NodeCloningResult VisitCleanupBehaviorDirectiveNode(IT4CleanupBehaviorDirective cleanupBehaviorDirectiveParam, [CanBeNull] Unit context) => CreateResult(new CleanupBehaviorDirective());
		public override T4NodeCloningResult VisitCodeNode(IT4Code codeParam, [CanBeNull] Unit context) => CreateResult(new Code());
		public override T4NodeCloningResult VisitCodeBlockNode(IT4CodeBlock codeBlockParam, [CanBeNull] Unit context) => throw new InvalidOperationException();
		public override T4NodeCloningResult VisitDirectiveNode(IT4Directive directiveParam, [CanBeNull] Unit context) => throw new InvalidOperationException();
		public override T4NodeCloningResult VisitDirectiveAttributeNode(IT4DirectiveAttribute directiveAttributeParam, [CanBeNull] Unit context) => CreateResult(new DirectiveAttribute());
		public override T4NodeCloningResult VisitEnvironmentVariableNode(IT4EnvironmentVariable environmentVariableParam, [CanBeNull] Unit context) => CreateResult(new EnvironmentVariable());
		public override T4NodeCloningResult VisitExpressionBlockNode(IT4ExpressionBlock expressionBlockParam, [CanBeNull] Unit context) => CreateResult(new ExpressionBlock());
		public override T4NodeCloningResult VisitFeatureBlockNode(IT4FeatureBlock featureBlockParam, [CanBeNull] Unit context) => CreateResult(new FeatureBlock());

		public override T4NodeCloningResult VisitFileNode(IT4File fileParam, [CanBeNull] Unit context)
		{
			if (!LexerSelector.HasCustomLexer(fileParam.LogicalPsiSourceFile))
			{
				return CreateResult(new File {LogicalPsiSourceFile = fileParam.LogicalPsiSourceFile});
			}

			var originalLexer = LexerSelector.SelectLexer(fileParam.LogicalPsiSourceFile);
			var parser = new T4Parser(originalLexer, fileParam.LogicalPsiSourceFile, PhysicalSourceFile, LexerSelector);
			var node = (IT4TreeNode) parser.ParseFile();
			return new T4NodeCloningResult(false, node);
		}

		public override T4NodeCloningResult VisitImportDirectiveNode(IT4ImportDirective importDirectiveParam, [CanBeNull] Unit context) =>
			CreateResult(new ImportDirective());

		public override T4NodeCloningResult VisitIncludeDirectiveNode(IT4IncludeDirective includeDirectiveParam, [CanBeNull] Unit context)
		{
			var result = new IncludeDirective();
			result.InitializeResolvedPath(includeDirectiveParam.ResolvedPath);
			return CreateResult(result);
		}

		public override T4NodeCloningResult VisitIncludedFileNode(IT4IncludedFile includedFileParam, [CanBeNull] Unit context)
		{
			if (!LexerSelector.HasCustomLexer(includedFileParam.LogicalPsiSourceFile))
			{
				return CreateResult(IncludedFile.FromOtherNodeNoChildren(includedFileParam));
			}

			var originalLexer = LexerSelector.SelectLexer(includedFileParam.LogicalPsiSourceFile);
			var parser = new T4Parser(originalLexer, includedFileParam.LogicalPsiSourceFile, PhysicalSourceFile, LexerSelector);
			var node = (IT4TreeNode) parser.BuildIncludedT4Tree(includedFileParam.LogicalPsiSourceFile);
			return  new T4NodeCloningResult(false, node);
		}

		public override T4NodeCloningResult VisitMacroNode(IT4Macro macroParam, [CanBeNull] Unit context) => CreateResult(new Macro());
		public override T4NodeCloningResult VisitOutputDirectiveNode(IT4OutputDirective outputDirectiveParam, [CanBeNull] Unit context) => CreateResult(new OutputDirective());
		public override T4NodeCloningResult VisitParameterDirectiveNode(IT4ParameterDirective parameterDirectiveParam, [CanBeNull] Unit context) => CreateResult(new ParameterDirective());
		public override T4NodeCloningResult VisitStatementBlockNode(IT4StatementBlock statementBlockParam, [CanBeNull] Unit context) => CreateResult(new StatementBlock());
		public override T4NodeCloningResult VisitTemplateDirectiveNode(IT4TemplateDirective templateDirectiveParam, [CanBeNull] Unit context) => CreateResult(new TemplateDirective());
		public override T4NodeCloningResult VisitUnknownDirectiveNode(IT4UnknownDirective unknownDirectiveParam, [CanBeNull] Unit context) => CreateResult(new UnknownDirective());
	}
}
