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
	}
}
