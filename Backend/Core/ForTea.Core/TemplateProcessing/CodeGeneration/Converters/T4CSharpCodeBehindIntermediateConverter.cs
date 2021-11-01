using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.GeneratorKind;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public class T4CSharpCodeBehindIntermediateConverter : T4CSharpIntermediateConverterBase
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

		private bool IsRoot { get; }

		public T4CSharpCodeBehindIntermediateConverter(
			[NotNull] IT4File file,
			[NotNull] IT4GeneratedClassNameProvider classNameProvider,
			bool isRoot = true
		) : this(file, classNameProvider, new T4PreprocessedGeneratorKind(), isRoot)
		{
		}

		protected T4CSharpCodeBehindIntermediateConverter(
			[NotNull] IT4File file,
			[NotNull] IT4GeneratedClassNameProvider classNameProvider,
			IT4GeneratorKind generatorKind,
			bool isRoot = true
		) : base(file, classNameProvider, generatorKind)
		{
			IsRoot = isRoot;
			if (IsRoot)
			{
				TransformTextMethodName = DefaultTransformTextMethodName;
				TransformTextAttributes = "";
			}
			else
			{
				TransformTextMethodName = DefaultTransformTextMethodName + Guid.NewGuid().ToString("N");
				TransformTextAttributes = "[__ReSharperSynthetic]";
			}
		}

		protected override string BaseClassResourceName => "GammaJul.ForTea.Core.Resources.TemplateBaseStub.cs";

		protected override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			bool hasHost
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

			Result.Append("        private ");
			description.AppendTypeMapped(Result);
			Result.Append(" ");
			description.AppendName(Result);
			Result.Append(" => ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
		}

		private void AppendDisabledInspections([NotNull] string inspection)
		{
			Result.Append("        // ReSharper disable ");
			Result.AppendLine(inspection);
		}

		protected override void AppendBaseClass(T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			if (!IsRoot) return;
			base.AppendBaseClass(intermediateResult);
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

		protected override string BaseClassDescription =>
			"    /// <summary>\n    /// Base class for this transformation\n    /// </summary>";

		protected override bool ShouldAppendPragmaDirectives => true;

		protected override string GetTransformTextOverridabilityModifier(bool hasCustomBaseClass)
		{
			if (!hasCustomBaseClass) return VirtualKeyword;
			return base.GetTransformTextOverridabilityModifier(true);
		}

		protected override string TransformTextMethodName { get; }
		protected override string TransformTextAttributes { get; }

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
