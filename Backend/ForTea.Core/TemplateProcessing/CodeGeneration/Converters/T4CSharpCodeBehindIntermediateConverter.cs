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
		private const string ToStringConversionPrefixText = "__To\x200CString(";
		public const string CodeCommentEndText = "/*_T4\x200CCodeEnd_*/";
		public const string CodeCommentStartText = "/*_T4\x200CCodeStart_*/";
		private const string GeneratedClassNameText = "Generated\x200CTransformation";

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

		protected override string ResourceName => "GammaJul.ForTea.Core.Resources.TemplateBaseStub.cs";

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
			if (description.HasSameSource(File))
			{
				var type = description.TypeToken;
				if (CSharpLexer.IsKeyword(type.GetText())) Result.Append("@");
				Result.AppendMapped(type);
			}
			else Result.Append(description.TypeString);

			Result.Append(" ");
			if (description.HasSameSource(File))
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

		protected override void AppendClasses(bool hostspecific)
		{
			AppendClass();
			AppendBaseClass();
			if (hostspecific) AppendHostInterface();
		}

		private void AppendHostInterface()
		{
			var provider = new T4TemplateResourceProvider(HostStubResourceName, this);
			Result.AppendLine(provider.ProcessResource());
		}

		protected override void AppendHost()
		{
			AppendIndent();
			Result.AppendLine(
				"public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
		}

		protected override string GeneratedClassName
		{
			get
			{
				var projectFile = File.GetSourceFile()?.ToProjectFile();
				if (projectFile == null) return GeneratedClassNameText;
				var dataManager = File.GetSolution().GetComponent<IT4ProjectModelTemplateDataManager>();
				var templateKind = dataManager.GetTemplateKind(projectFile);
				if (templateKind != T4TemplateKind.Preprocessed) return GeneratedClassNameText;
				string fileName = File.GetSourceFile()?.Name.WithoutExtension();
				if (fileName == null) return GeneratedClassNameText;
				if (!ValidityChecker.IsValidIdentifier(fileName)) return GeneratedClassNameText;
				return fileName;
			}
		}

		// No indents should be inserted in code-behind file in order to avoid indenting code in code blocks
		protected override void AppendIndent(int size)
		{
		}

		#region IT4ElementAppendFormatProvider
		public override string ToStringConversionPrefix => ToStringConversionPrefixText;
		public override string ToStringConversionSuffix => ")";
		public override string ExpressionWritingPrefix => "this.Write(";
		public override string ExpressionWritingSuffix => ");";
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
