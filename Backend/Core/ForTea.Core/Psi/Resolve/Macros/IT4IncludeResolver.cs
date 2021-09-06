using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public interface IT4IncludeResolver
	{
		[NotNull]
		VirtualFileSystemPath ResolvePath([NotNull] T4ResolvedPath path);

		[CanBeNull]
		IPsiSourceFile Resolve([NotNull] T4ResolvedPath path);
	}
}
