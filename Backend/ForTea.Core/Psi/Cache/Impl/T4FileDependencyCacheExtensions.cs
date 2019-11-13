using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	public static class T4FileDependencyCacheExtensions
	{
		[NotNull]
		private static Key<object> IndirectDependencyInvalidationMarker { get; } =
			new Key<object>("IndirectDependencyInvalidationMarker");

		/// <summary>
		/// Marks that the file is being invalidated due to indirect dependency change.
		/// It would be most logical to use boolean here but value types cannot be used as user data.
		/// </summary>
		[NotNull]
		private static object Marker { get; } = new object();

		public static bool IsIndirectDependency([NotNull] this IPsiSourceFile sourceFile) =>
			sourceFile.GetData(IndirectDependencyInvalidationMarker) != null;

		public static void MarkAsIndirectDependency([NotNull] this IPsiSourceFile sourceFile) =>
			sourceFile.PutData(IndirectDependencyInvalidationMarker, Marker);

		public static void MarkAsIndependent([NotNull] this IPsiSourceFile sourceFile) =>
			sourceFile.PutData(IndirectDependencyInvalidationMarker, null);
	}
}
