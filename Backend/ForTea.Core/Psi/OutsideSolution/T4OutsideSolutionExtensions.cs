using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.OutsideSolution
{
	internal static class T4OutsideSolutionExtensions
	{
		[NotNull]
		private static Key<VirtualFileSystemPath> OutsideSolutionPathKey { get; } =
			new Key<VirtualFileSystemPath>("OutsideSolutionPath");

		public static void SetOutsideSolutionPath(
			[NotNull] this IDocument document,
			[NotNull] VirtualFileSystemPath fileSystemPath
		) => document.PutData(OutsideSolutionPathKey, fileSystemPath);

		[NotNull]
		public static VirtualFileSystemPath GetOutsideSolutionPath([NotNull] this IDocument document) =>
			document.GetData(OutsideSolutionPathKey) ?? VirtualFileSystemPath.GetEmptyPathFor(InteractionContext.SolutionContext);
	}
}
