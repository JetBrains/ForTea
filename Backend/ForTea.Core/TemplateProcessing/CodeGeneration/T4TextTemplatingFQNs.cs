using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	public static class T4TextTemplatingFQNs
	{
		[NotNull] public const string HostInterface =
			"global::Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost";

		[NotNull] public const string HostImpl =
			"global::Microsoft.VisualStudio.TextTemplating.JetBrains.TextTemplatingEngineHost";

		[NotNull] public const string Macros = "global::System.Collections.Generic.Dictionary<string, string>";

		[NotNull] public const string TextTransformation =
			"global::Microsoft.VisualStudio.TextTemplating." +
			T4CSharpIntermediateConverterBase.GeneratedBaseClassNameString;

		[NotNull] public const string ToStringHelper =
			"global::Microsoft.VisualStudio.TextTemplating.ToStringHelper";

		public const string Lifetime = "global::JetBrains.Lifetimes.Lifetime";
	}
}
