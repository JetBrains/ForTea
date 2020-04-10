using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	public interface IT4PsiFileSelector
	{
		IPsiSourceFile FindMostSuitableFile(
			[NotNull] FileSystemPath path,
			[NotNull] IPsiSourceFile requester
		);
	}
}
