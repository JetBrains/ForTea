using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public abstract class T4CSharpCodeBehindIntermediateConverterBase : T4CSharpIntermediateConverterBase
	{
		[NotNull] public const string CodeCommentStartText = "/*_T4\x200CCodeStart_*/";
		[NotNull] public const string CodeCommentEndText = "/*_T4\x200CCodeEnd_*/";
		[NotNull] public const string ExpressionCommentStartText = "/*_T4\x200CExpressionStart*/";
		[NotNull] public const string ExpressionCommentEndText = "/*_T4\x200CExpressionEnd*/";

		[NotNull, ItemNotNull]
		private IEnumerable<string> DisabledPropertyInspections { get; } = new[]
		{
			"BuiltInTypeReferenceStyle",
			"RedundantNameQualifier"
		};

		protected T4CSharpCodeBehindIntermediateConverterBase(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult,
			[NotNull] IT4File file
		) : base(intermediateResult, file)
		{
		}

		protected override string BaseClassResourceName => "GammaJul.ForTea.Core.Resources.TemplateBaseStub.cs";

		protected override void AppendSyntheticAttribute()
		{
			AppendIndent();
			Result.AppendLine($"[{SyntheticAttribute.Name}]");
		}

		protected override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			// There's no need to initialize parameters in code-behind since this code is never displayed anyway 
		}

		protected override void AppendParameterDeclaration(T4ParameterDescription description)
		{
			foreach (string inspection in DisabledPropertyInspections)
			{
				AppendDisabledInspections(inspection);
			}

			Result.Append("        private global::");
			var type = description.TypeToken;
			if (CSharpLexer.IsKeyword(type.GetText())) Result.Append("@");
			Result.AppendMapped(type);

			Result.Append(" ");
			var name = description.NameToken;
			if (CSharpLexer.IsKeyword(name.GetText())) Result.Append("@");
			Result.AppendMapped(name);

			Result.Append(" => ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
		}

		private void AppendDisabledInspections([NotNull] string inspection)
		{
			Result.Append("        // ReSharper disable ");
			Result.AppendLine(inspection);
		}

		protected override void AppendHost()
		{
			AppendIndent();
			Result.AppendLine(
				"public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
		}

		// No indents should be inserted in code-behind file in order to avoid indenting code in code blocks
		protected override void AppendIndent(int size)
		{
		}

		protected override bool ShouldAppendPragmaDirectives => true;

		#region IT4ElementAppendFormatProvider
		public override string CodeCommentStart => CodeCommentStartText;
		public override string CodeCommentEnd => CodeCommentEndText;
		public override string ExpressionCommentStart => ExpressionCommentStartText;
		public override string ExpressionCommentEnd => ExpressionCommentEndText;
		public override string Indent => "";
		public override bool ShouldBreakExpressionWithLineDirective => false;

		public override void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, IT4TreeNode node)
		{
		}

		public override void AppendLineDirective(T4CSharpCodeGenerationResult destination, IT4TreeNode node)
		{
			// Line directives in code-behind affect nothing anyway.
			// The mapping between the code and the document
			// is handled by document range translators.
		}

		public override void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code) =>
			destination.AppendMapped(code);
		#endregion
	}
}
