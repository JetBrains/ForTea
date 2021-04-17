using System;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing.Parser
{
	internal sealed class T4CloningParserVisitor : TreeNodeVisitor
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

		public override void VisitAssemblyDirectiveNode(IT4AssemblyDirective assemblyDirectiveParam)
		{
			var result = new AssemblyDirective();
			result.InitializeResolvedPath(assemblyDirectiveParam.ResolvedPath);
			CurrentClone = result;
		}

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
		public override void VisitFileNode(IT4File fileParam) => CurrentClone = new File {LogicalPsiSourceFile = fileParam.LogicalPsiSourceFile};
		public override void VisitImportDirectiveNode(IT4ImportDirective importDirectiveParam) => CurrentClone = new ImportDirective();
		public override void VisitIncludeDirectiveNode(IT4IncludeDirective includeDirectiveParam)
		{
			var result = new IncludeDirective();
			result.InitializeResolvedPath(includeDirectiveParam.ResolvedPath);
			CurrentClone = result;
		}

		public override void VisitIncludedFileNode(IT4IncludedFile includedFileParam) => CurrentClone = IncludedFile.FromOtherNodeNoChildren(includedFileParam);
		public override void VisitMacroNode(IT4Macro macroParam) => CurrentClone = new Macro();
		public override void VisitOutputDirectiveNode(IT4OutputDirective outputDirectiveParam) => CurrentClone = new OutputDirective();
		public override void VisitParameterDirectiveNode(IT4ParameterDirective parameterDirectiveParam) => CurrentClone = new ParameterDirective();
		public override void VisitStatementBlockNode(IT4StatementBlock statementBlockParam) => CurrentClone = new StatementBlock();
		public override void VisitTemplateDirectiveNode(IT4TemplateDirective templateDirectiveParam) => CurrentClone = new TemplateDirective();
		public override void VisitUnknownDirectiveNode(IT4UnknownDirective unknownDirectiveParam) => CurrentClone = new UnknownDirective();
	}
}
