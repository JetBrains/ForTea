using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public abstract class T4MacroResolverBase : IT4MacroResolver
	{
		public virtual bool IsSupported(IT4Macro macro) => true;

		public IReadOnlyDictionary<string, string> ResolveHeavyMacros(IEnumerable<string> macros, IProjectFile file)
		{
			var requestedMacros = macros.ToList();
			var resolvedLightMacros = ResolveAllLightMacrosInternal(file);
			var requestedHeavyMacros = requestedMacros.Where(it => !resolvedLightMacros.ContainsKey(it)).AsList();
			if (!requestedHeavyMacros.Any()) return resolvedLightMacros;
			var resolvedHeavyMacros = ResolveOnlyHeavyMacros(requestedHeavyMacros, file);
			resolvedLightMacros.AddRange(resolvedHeavyMacros);
			return resolvedLightMacros;
		}

		public IReadOnlyDictionary<string, string> ResolveAllLightMacros(IProjectFile file) =>
			ResolveAllLightMacrosInternal(file);

		[NotNull]
		protected abstract Dictionary<string, string> ResolveAllLightMacrosInternal([NotNull] IProjectFile file);

		[NotNull]
		protected abstract IReadOnlyDictionary<string, string> ResolveOnlyHeavyMacros(
			[NotNull, ItemNotNull] IList<string> macros,
			[NotNull] IProjectFile file);
	}
}
