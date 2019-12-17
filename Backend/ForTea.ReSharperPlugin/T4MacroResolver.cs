using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Macros;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.EditorConfig;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace JetBrains.ForTea.ReSharperPlugin
{
	[SolutionComponent]
	public sealed class T4MacroResolver : T4MacroResolverBase
	{
		[NotNull]
		private T4MacroResolutionCache Cache { get; }

		public T4MacroResolver([NotNull] T4MacroResolutionCache cache) => Cache = cache;

		[NotNull]
		private static Dictionary<string, string> ourEmptyDictionary { get; } = new Dictionary<string, string>();

		protected override Dictionary<string, string> ResolveAllLightMacrosInternal(IProjectFile file) =>
			ourEmptyDictionary;

		protected override IReadOnlyDictionary<string, string> ResolveOnlyHeavyMacros(
			IList<string> macros,
			IProjectFile file
		) => Cache
			.Map.TryGetValue(file.ToSourceFile().NotNull())
			?.ResolvedMacros
			.Where(it => macros.Contains(it.Key))
			.ToDictionary() ?? ourEmptyDictionary;
	}
}
