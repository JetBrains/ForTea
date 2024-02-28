using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl
{
  [SolutionComponent(Instantiation.DemandAnyThreadSafe)]
  [DerivedComponentsInstantiationRequirement(InstantiationRequirement.None)]
  public class T4BasicMacroResolver : IT4MacroResolver
  {
    [NotNull]
    protected static Dictionary<string, string> ourEmptyDictionary { get; } = new Dictionary<string, string>();

    public virtual IReadOnlyDictionary<string, string>
      ResolveHeavyMacros(IEnumerable<string> macros, IProjectFile file) =>
      ourEmptyDictionary;

    public virtual bool IsSupported(IT4Macro macro) => true;
  }
}