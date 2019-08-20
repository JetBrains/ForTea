using System;
using GammaJul.ForTea.Core.Daemon.Attributes.GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Syntax
{
	public sealed class T4SyntaxHighlightingProcessor : SyntaxHighlightingProcessor
	{
		public override bool InteriorShouldBeProcessed(ITreeNode element, IHighlightingConsumer context)
		{
			var type = element.NodeType;
			if (type == ElementType.T4_MACRO) return false;
			if (type == ElementType.T4_ENVIRONMENT_VARIABLE) return false;
			return true;
		}

		public override void ProcessBeforeInterior(ITreeNode element, IHighlightingConsumer context)
		{
			var type = element.NodeType;
			if (type == ElementType.T4_MACRO) context.AddHighlighting(CreateMacroHighlighting((IT4Macro) element));
			else if (type == ElementType.T4_ENVIRONMENT_VARIABLE)
				context.AddHighlighting(CreateEnvironmentVariableHighlighting((IT4EnvironmentVariable) element));
			else if (type == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE)
			{
				const string id = T4HighlightingAttributeIds.RAW_ATTRIBUTE_VALUE;
				var highlighting = new ReSharperSyntaxHighlighting(id, null, element.GetDocumentRange());
				context.AddHighlighting(highlighting);
			}
		}

		[CanBeNull]
		private string ExpandEnvironmentVariable([NotNull] IT4EnvironmentVariable variable)
		{
			var value = variable.RawAttributeValue;
			if (value == null) return null;
			return Environment.GetEnvironmentVariable(value.GetText());
		}

		[CanBeNull]
		private string ExpandMacro([NotNull] IT4Macro macro)
		{
			var projectFile = macro.GetSourceFile().NotNull().ToProjectFile().NotNull();
			var solution = projectFile.GetSolution();
			string name = macro.RawAttributeValue?.GetText();
			if (name == null) return null;
			var macros = solution.GetComponent<IT4MacroResolver>().Resolve(new[] {name}, projectFile);
			return macros.ContainsKey(name) ? macros[name] : null;
		}

		[NotNull]
		private IHighlighting CreateMacroHighlighting(IT4Macro element)
		{
			string expanded = ExpandMacro(element);
			if (expanded == null) return new T4UnresolvedMacroHighlighting(element);
			const string id = T4HighlightingAttributeIds.MACRO;
			string message = expanded.WithPrefix("macro");
			var range = element.GetDocumentRange();
			return new ReSharperSyntaxHighlighting(id, message, range);
		}

		private IHighlighting CreateEnvironmentVariableHighlighting(IT4EnvironmentVariable element)
		{
			string expanded = ExpandEnvironmentVariable(element);
			if (expanded == null) return new T4UnresolvedEnvironmentVariableHighlighting(element);
			const string id = T4HighlightingAttributeIds.ENVIRONMENT_VARIABLE;
			var range = element.GetDocumentRange();
			string message = expanded.WithPrefix("environment variable");
			return new ReSharperSyntaxHighlighting(id, message, range);
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
