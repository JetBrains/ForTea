using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
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
  public sealed class T4ReSharperMacroResolver : T4BasicMacroResolver
  {
    [NotNull] private T4MacroResolutionCache Cache { get; }

    public T4ReSharperMacroResolver([NotNull] T4MacroResolutionCache cache) => Cache = cache;

    public override IReadOnlyDictionary<string, string> ResolveHeavyMacros(
      IEnumerable<string> macros,
      IProjectFile file
    ) => Cache
      .Map.TryGetValue(file.ToSourceFile().NotNull())
      ?.ResolvedMacros
      .Where(it => macros.Contains(it.Key))
      .ToDictionary() ?? ourEmptyDictionary;
  }
}