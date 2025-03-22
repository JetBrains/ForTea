using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders
{
  [DerivedComponentsInstantiationRequirement(InstantiationRequirement.DeadlockSafe)]
  internal interface IT4BlockSpaceTypeProvider
  {
    [Pure]
    SpaceType? Provide([NotNull] CSharpFmtStageContext ctx);
  }
}