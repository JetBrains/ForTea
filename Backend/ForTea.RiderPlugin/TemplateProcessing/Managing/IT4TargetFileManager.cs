using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TargetFileManager
	{
		[NotNull]
		string GetExpectedTargetFileName([NotNull] IT4File file);

		[NotNull]
		FileSystemPath GetTemporaryExecutableLocation([NotNull] IT4File file);

		[NotNull]
		FileSystemPath GetExpectedTemporaryTargetFileLocation([NotNull] IT4File file);

		[NotNull]
		FileSystemPath SaveExecutionResults([NotNull] IT4File file);

		[NotNull]
		FileSystemPath SavePreprocessResults([NotNull] IT4File file, [NotNull] string text);
	}
}
