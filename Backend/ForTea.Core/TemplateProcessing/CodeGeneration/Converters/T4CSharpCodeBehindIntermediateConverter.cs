using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpCodeBehindIntermediateConverter : T4CSharpIntermediateConverterBase
	{
		[NotNull] public const string GeneratedClassNameString = "Generated\x200CTransformation";
		[NotNull] public const string GeneratedBaseClassNameString = GeneratedClassNameString + "Base";
		[NotNull] private const string HostStubResourceName = "GammaJul.ForTea.Core.Resources.HostStub.cs";

		[NotNull, ItemNotNull] private string[] DisabledPropertyInspections =
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
		protected override string GeneratedClassName => GeneratedClassNameString;
		protected override string GeneratedBaseClassName => GeneratedBaseClassNameString;

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
			if (description.IsVisible)
			{
				var type = description.TypeToken;
				if (CSharpLexer.IsKeyword(type.GetText())) Result.Append("@");
				Result.AppendMapped(type);
			}
			else Result.Append(description.TypeString);

			Result.Append(" ");
			if (description.IsVisible)
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

		// No indents should be inserted in code-behind file in order to avoid indenting code in code blocks
		protected override void AppendIndent(int size)
		{
		}
	}
}
