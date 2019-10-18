using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpCodeBehindIntermediateConverter : T4CSharpIntermediateConverterBase
	{
		[NotNull] private const string HostStubResourceName = "GammaJul.ForTea.Core.Resources.HostStub.cs";
		public const string CodeCommentEndText = "/*_T4\x200CCodeEnd_*/";
		public const string CodeCommentStartText = "/*_T4\x200CCodeStart_*/";

		[NotNull, ItemNotNull]
		private IEnumerable<string> DisabledPropertyInspections { get; } = new[]
		{
			"BuiltInTypeReferenceStyle",
			"RedundantNameQualifier"
		};

		public T4CSharpCodeBehindIntermediateConverter(
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
			if (description.HasSameSourceFile(File.GetSourceFile()))
			{
				var type = description.TypeToken;
				if (CSharpLexer.IsKeyword(type.GetText())) Result.Append("@");
				Result.AppendMapped(type);
			}
			else Result.Append(description.TypeString);

			Result.Append(" ");
			if (description.HasSameSourceFile(File.GetSourceFile()))
			{
				var name = description.NameToken;
				if (CSharpLexer.IsKeyword(name.GetText())) Result.Append("@");
				Result.AppendMapped(name);
			}
			else Result.Append(description.NameString);

			Result.Append(" => ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
		}

		private void AppendDisabledInspections([NotNull] string inspection)
		{
			Result.Append("        // ReSharper disable ");
			Result.AppendLine(inspection);
		}

		protected override void AppendNamespacePrefix()
		{
			if (!IntermediateResult.HasHost) return;
			Result.AppendLine(new T4TemplateResourceProvider(HostStubResourceName).ProcessResource());
		}

		protected override void AppendHost()
		{
			AppendIndent();
			Result.AppendLine(
				"public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
		}

		[CanBeNull]
		private string TryGetGeneratedClassNameFromFile()
		{
			var projectFile = File.GetSourceFile()?.ToProjectFile();
			if (projectFile == null) return null;
			var dataManager = File.GetSolution().GetComponent<IT4TemplateKindProvider>();
			if (!dataManager.IsPreprocessedTemplate(projectFile)) return null;
			string fileName = File.GetSourceFile()?.Name.WithoutExtension();
			if (fileName == null) return null;
			if (!ValidityChecker.IsValidIdentifier(fileName)) return null;
			return fileName;
		}

		protected override string GeneratedClassName => TryGetGeneratedClassNameFromFile() ?? GeneratedClassNameString;

		protected override string GeneratedBaseClassName
		{
			get
			{
				string name = TryGetGeneratedClassNameFromFile();
				if (name == null) return GeneratedClassNameString;
				return name + "Base";
			}
		}

		// No indents should be inserted in code-behind file in order to avoid indenting code in code blocks
		protected override void AppendIndent(int size)
		{
		}

		protected override bool ShouldAppendPragmaDirectives => true;

		#region IT4ElementAppendFormatProvider
		public override string CodeCommentStart => CodeCommentStartText;
		public override string CodeCommentEnd => CodeCommentEndText;
		public override string Indent => "";
		public override bool ShouldBreakExpressionWithLineDirective => false;

		public override void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, Int32<DocColumn> offset)
		{
		}

		public override void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code) =>
			destination.AppendMapped(code);
		#endregion
	}
}
