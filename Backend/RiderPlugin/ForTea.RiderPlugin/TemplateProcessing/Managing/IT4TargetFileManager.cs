using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TargetFileManager
	{
		[NotNull]
		VirtualFileSystemPath GetTemporaryExecutableLocation([NotNull] IT4File file);

		[NotNull]
		VirtualFileSystemPath GetExpectedTemporaryTargetFileLocation([NotNull] IT4File file);

		void TryProcessExecutionResults([NotNull] IT4File file);

		void SavePreprocessResults([NotNull] IT4File file, [NotNull] string text);
	}
}
