using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services
{
	public static class T4TemplateManagerConstants
	{
		[NotNull] private const string Preprocessor = "TextTemplatingFilePreprocessor";
		[NotNull] private const string Generator = "TextTemplatingFileGenerator";

		public static T4TemplateKind ToTemplateKind([CanBeNull] string value)
		{
			switch (value)
			{
				case Preprocessor: return T4TemplateKind.Preprocessed;
				case Generator: return T4TemplateKind.Executable;
				default: return T4TemplateKind.Unknown;
			}
		}

		[NotNull]
		public static string ToRawValue(T4TemplateKind kind)
		{
			switch (kind)
			{
				case T4TemplateKind.Executable: return Generator;
				case T4TemplateKind.Preprocessed: return Preprocessor;
				default: return "";
			}
		}
	}
}
