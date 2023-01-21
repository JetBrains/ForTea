using System.Collections.Generic;
using JetBrains.Annotations;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Macros
{
  public sealed class T4MacroResolutionData
  {
    [NotNull] public IReadOnlyDictionary<string, string> ResolvedMacros { get; }

    public T4MacroResolutionData([NotNull] IReadOnlyDictionary<string, string> resolvedMacros) =>
      ResolvedMacros = resolvedMacros;
  }
}