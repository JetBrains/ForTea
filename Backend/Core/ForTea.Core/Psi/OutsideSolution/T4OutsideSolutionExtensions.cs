using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
	internal static class T4OutsideSolutionExtensions
	{
		[NotNull]
		private static Key<FileSystemPath> OutsideSolutionPathKey { get; } =
			new Key<FileSystemPath>("OutsideSolutionPath");

		public static void SetOutsideSolutionPath(
			[NotNull] this IDocument document,
			[NotNull] FileSystemPath fileSystemPath
		) => document.PutData(OutsideSolutionPathKey, fileSystemPath);

		[NotNull]
		public static FileSystemPath GetOutsideSolutionPath([NotNull] this IDocument document) =>
			document.GetData(OutsideSolutionPathKey) ?? FileSystemPath.Empty;
	}
}
