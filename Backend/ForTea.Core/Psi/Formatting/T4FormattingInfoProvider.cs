using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	[Language(typeof(T4Language))]
	public class T4FormattingInfoProvider :
		FormatterInfoProviderWithFluentApi<CodeFormattingContext, T4FormatterSettingsKey>
	{
		public T4FormattingInfoProvider(ISettingsSchema settingsSchema) : base(settingsSchema)
		{
		}

		public override void ModifyIndent(
			ITreeNode nodeToIndent,
			ref Whitespace? indent,
			CodeFormattingContext context,
			IIndentingStage<T4FormatterSettingsKey> callback,
			IndentType indentType
		) => indent = Whitespace.Empty;

		public override ProjectFileType MainProjectFileType => T4ProjectFileType.Instance;
	}
}
