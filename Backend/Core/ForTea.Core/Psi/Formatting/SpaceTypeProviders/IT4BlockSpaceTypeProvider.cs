using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders
{
  internal interface IT4BlockSpaceTypeProvider
  {
    [Pure]
    SpaceType? Provide([NotNull] CSharpFmtStageContext ctx);
  }
}