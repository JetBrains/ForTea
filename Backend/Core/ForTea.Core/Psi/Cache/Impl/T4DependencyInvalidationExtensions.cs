using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	public static class T4DependencyInvalidationExtensions
	{
		public static bool IsBeingIndirectlyUpdated([NotNull] this IPsiSourceFile file) => file.GetData(Key) != null;

		public static void SetBeingIndirectlyUpdated([NotNull] this IPsiSourceFile file, bool value) =>
			file.PutData(Key, value ? OurOutdatedDependencyIndicator : null);

		[NotNull]
		private static object OurOutdatedDependencyIndicator { get; } = new object();

		[NotNull]
		private static Key<object> Key { get; } = new Key<object>("T4 outdated dependency indicator");
	}
}
