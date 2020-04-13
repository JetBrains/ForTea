using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public interface IT4IncludeResolver
	{
		[NotNull]
		FileSystemPath ResolvePath([NotNull] IT4PathWithMacros path);

		[CanBeNull]
		IPsiSourceFile Resolve([NotNull] IT4PathWithMacros path);
	}
}
