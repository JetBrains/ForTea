using JetBrains.Annotations;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl
{
	public static class T4AssemblyResolveUtils
	{
		[NotNull]
		public static AssemblyReferenceTarget ToAssemblyReferenceTarget([NotNull] this FileSystemPath path) =>
			new AssemblyReferenceTarget(AssemblyNameInfo.Empty, path);

		[NotNull]
		public static AssemblyReferenceTarget ToAssemblyReferenceTarget([NotNull] this AssemblyNameInfo info) =>
			new AssemblyReferenceTarget(info, FileSystemPath.Empty);
	}
}
