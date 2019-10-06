using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Invalidation
{
	public interface IT4IndirectIncludeInvalidator
	{
		void InvalidateIndirectIncludes([NotNull] FileSystemPath updatedFile);
	}
}
