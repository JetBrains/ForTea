using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders.Impl
{
	internal sealed class T4ExpressionBlockOuterBoundSpaceTypeProvider : T4BlockSpaceTypeProviderBase
	{
		protected override bool IsApplicable(CSharpFmtStageContext ctx) =>
			IsNearExpressionBlockLeftBoundary(ctx) || IsNearExpressionBlockRightBoundary(ctx);

		protected override SpaceType Type => SpaceType.NoSpace;

		private static bool IsNearExpressionBlockLeftBoundary([NotNull] CSharpFmtStageContext ctx)
		{
			if (!(ctx.RightChild is ICommentNode comment)) return false;
			return comment.GetText() == T4CSharpCodeBehindIntermediateConverter.ExpressionCommentStartText;
		}

		private static bool IsNearExpressionBlockRightBoundary([NotNull] CSharpFmtStageContext ctx)
		{
			if (!(ctx.LeftChild is ICommentNode comment)) return false;
			return comment.GetText() == T4CSharpCodeBehindIntermediateConverter.ExpressionCommentEndText;
		}
	}
}
