using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	/// <summary>
	/// T4 files that are included into preprocessed files
	/// should be handled differently from the ones that are not.
	/// However, that knowledge is required during PSI module construction,
	/// and on that stage there's no guarantee of caches being ready.
	/// Therefore, we store this information directly in project files.
	/// </summary>
	public static class T4PreprocessedTemplateFlagUtils
	{
		private static Key<object> PreprocessedKey { get; } = new Key<object>("T4_INCLUDED_INTO_PREPROCESSED_FILE_KEY");
		public static void FlagAsPreprocessed([NotNull] this IProjectFile thіs) => thіs.PutKey(PreprocessedKey);
		public static void FlagAsExecutable([NotNull] this IProjectFile thіs) => thіs.RemoveKey(PreprocessedKey);
		public static bool IsFlaggedAsPreprocessed([NotNull] this IProjectFile thіs) => thіs.HasKey(PreprocessedKey);
	}
}
