using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	public interface IT4FileDependencyGraph
	{
		/// <summary>
		/// This is used for building correct PSI for .ttinclude files.
		/// T4 includes are similar to C++ ones, and symbols used in them
		/// can be defined in other .ttinclude files.
		/// It is only possible to track this by choosing the most complete context for each file,
		/// i.e. the uppermost file that includes the current one.
		/// </summary>
		/// TODO: cache root?
		[NotNull]
		IPsiSourceFile FindBestRoot([NotNull] IPsiSourceFile file);
	}
}
