using GammaJul.ForTea.Core.Psi.Utils;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public interface IT4PathWithMacros
	{
		[NotNull]
		string ResolveString();

		[NotNull]
		FileSystemPath ResolvePath();

		[CanBeNull]
		IPsiSourceFile Resolve();

		[CanBeNull]
		IT4File ResolveT4File([NotNull] T4IncludeGuard guard);

		bool IsEmpty { get; }
	}
}
