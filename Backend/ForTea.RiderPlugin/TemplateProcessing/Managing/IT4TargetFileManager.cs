using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TargetFileManager
	{
		string GetTargetFileName([NotNull] IT4File file, [CanBeNull] string targetExtension = null);

		[NotNull]
		FileSystemPath GetTemporaryExecutableLocation([NotNull] IT4File file);

		[NotNull]
		FileSystemPath SaveResults(
			IT4ExecutionResult result,
			[NotNull] IT4File file,
			[CanBeNull] string targetExtension = null);
	}
}
