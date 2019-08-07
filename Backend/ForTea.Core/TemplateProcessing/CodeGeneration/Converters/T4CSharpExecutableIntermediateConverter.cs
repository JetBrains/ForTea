using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpExecutableIntermediateConverter : T4CSharpIntermediateConverter
	{
		[NotNull] private const string SuffixResource =
			"GammaJul.ForTea.Core.Resources.TemplateBaseFullExecutableSuffix.cs";

		[NotNull] private const string HostResource = "GammaJul.ForTea.Core.Resources.Host.cs";

		public T4CSharpExecutableIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult,
			[NotNull] IT4File file
		) : base(intermediateResult, file)
		{
		}

		// Avoid name clash
		protected override string GeneratedClassName =>
			"__" + base.GeneratedClassName + "__" + DefaultGeneratedClassName + "__";

		// When creating executable, it is better to put base class first,
		// to make error messages more informative
		protected override void AppendClasses(bool hostspecific)
		{
			AppendBaseClass();
			AppendMainContainer();
			if (hostspecific) AppendHostDefinition();
			AppendClass();
		}

		protected override void AppendHost()
		{
			AppendIndent();
			Result.AppendLine(
				"public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; } =");
			AppendIndent();
			Result.AppendLine("    new Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost();");
		}

		private void AppendHostDefinition()
		{
			var provider = new T4TemplateResourceProvider(HostResource, this);
			string host = provider.ProcessResource(File.GetSourceFile().GetLocation().FullPath);
			Result.Append(host);
		}

		private void AppendMainContainer()
		{
			var provider = new T4TemplateResourceProvider(SuffixResource, this);
			string suffix = provider.ProcessResource(GeneratedClassName, IntermediateResult.Encoding);
			Result.Append(suffix);
		}

		protected override void AppendImports()
		{
			base.AppendImports();
			AppendIndent();
			Result.AppendLine("using System.IO;");
			AppendIndent();
			Result.AppendLine("using System.Text;");
		}

		protected override IT4ElementAppendFormatProvider Provider =>
			new T4RealCodeFormatProvider(new string(' ', CurrentIndent * 4));
	}
}
