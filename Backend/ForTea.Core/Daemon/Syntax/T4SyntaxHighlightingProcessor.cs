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

		[CanBeNull]
		private static string ExpandEnvironmentVariable([NotNull] IT4EnvironmentVariable variable)
		{
			var value = variable.RawAttributeValue;
			if (value == null) return null;
			return Environment.GetEnvironmentVariable(value.GetText());
		}

		[CanBeNull]
		private static string ExpandMacro([NotNull] IT4Macro macro)
		{
			var projectFile = macro.GetSourceFile().NotNull().ToProjectFile().NotNull();
			var solution = projectFile.GetSolution();
			string name = macro.RawAttributeValue?.GetText();
			if (name == null) return null;
			var macros = solution.GetComponent<IT4MacroResolver>().Resolve(new[] {name}, projectFile);
			return macros.ContainsKey(name) ? macros[name] : null;
		}

		private static void HighlightUnresolvedMacro(
			[NotNull] IT4Macro macro,
			[NotNull] IHighlightingConsumer context
		)
		{
			var projectFile = macro.GetSourceFile().NotNull().ToProjectFile().NotNull();
			var solution = projectFile.GetSolution();
			if (solution.GetComponent<IT4MacroResolver>().IsSupported(macro))
				context.AddHighlighting(new T4UnresolvedMacroHighlighting(macro));
			else context.AddHighlighting(new T4UnsupportedMacroHighlighting(macro));
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
			string expanded = ExpandMacro(element);
			if (expanded == null)
			{
				HighlightUnresolvedMacro(element, context);
				return;
			}

			const string id = T4HighlightingAttributeIds.MACRO;
			string message = expanded.WithPrefix("macro");
			var range = value.GetDocumentRange();
			context.AddHighlighting(new ReSharperSyntaxHighlighting(id, message, range));
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
			string expanded = ExpandEnvironmentVariable(element);
			if (expanded == null)
			{
				context.AddHighlighting(new T4UnresolvedEnvironmentVariableHighlighting(element));
				return;
			}

			const string id = T4HighlightingAttributeIds.ENVIRONMENT_VARIABLE;
			var range = value.GetDocumentRange();
			string message = expanded.WithPrefix("environment variable");
			context.AddHighlighting(new ReSharperSyntaxHighlighting(id, message, range));
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
