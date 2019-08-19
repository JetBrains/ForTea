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

		/// <note>
		/// This method performs write operations without checking write lock.
		/// That is done to use it inside ISingleFileCustomTool
		/// </note>
		[NotNull]
		FileSystemPath CopyExecutionResults([NotNull] IT4File file);

		void UpdateProjectModel([NotNull] IT4File file, [NotNull] FileSystemPath result);

		[NotNull]
		FileSystemPath SavePreprocessResults([NotNull] IT4File file, [NotNull] string text);
	}
}
