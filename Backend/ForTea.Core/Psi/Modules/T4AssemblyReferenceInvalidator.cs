using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.Modules.References;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	public static class T4AssemblyReferenceInvalidator
	{
		public static bool InvalidateAssemblies(
			[NotNull] T4DeclaredAssembliesDiff dataDiff,
			[NotNull] T4AssemblyReferenceManager referenceManager
		)
		{
			bool result = false;
			// removes the assembly references from the old assembly directives
			foreach (var _ in dataDiff.RemovedAssemblies.Where(referenceManager.TryRemoveReference))
			{
				result = true;
			}

			// adds assembly references from the new assembly directives
			foreach (var _ in dataDiff.AddedAssemblies.SelectNotNull(referenceManager.TryAddReference))
			{
				result = true;
			}

			return result;
		}
	}
}
