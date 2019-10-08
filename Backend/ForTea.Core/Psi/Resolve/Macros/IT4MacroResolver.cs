using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public interface IT4MacroResolver
	{
		[NotNull]
		IReadOnlyDictionary<string, string> ResolveHeavyMacros(
			[NotNull, ItemNotNull] IEnumerable<string> macros,
			[NotNull] IProjectFile file
		);

		[NotNull]
		IReadOnlyDictionary<string, string> ResolveAllLightMacros([NotNull] IProjectFile file);

		// TODO: move somewhere else. Wtf Macro resolver handles assemblies?
		void InvalidateAssemblies(
			[NotNull] T4DeclaredAssembliesDiff dataDiff,
			ref bool hasChanges,
			[NotNull] IProjectFile file,
			[NotNull] T4AssemblyReferenceManager referenceManager
		);

		bool IsSupported([NotNull] IT4Macro macro);
	}
}
