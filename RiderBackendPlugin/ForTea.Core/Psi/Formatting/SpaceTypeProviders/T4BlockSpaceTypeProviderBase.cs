using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders
{
	internal abstract class T4BlockSpaceTypeProviderBase : IT4BlockSpaceTypeProvider
	{
		public SpaceType? Provide(CSharpFmtStageContext ctx)
		{
			if (!IsApplicable(ctx)) return null;
			return Type;
		}

		protected abstract bool IsApplicable([NotNull] CSharpFmtStageContext ctx);
		protected abstract SpaceType Type { get; }
	}
}
