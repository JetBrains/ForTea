using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders
{
	internal abstract class T4BlockInnerBoundSpaceTypeProviderBase : T4BlockSpaceTypeProviderBase
	{
		protected override bool IsApplicable(CSharpFmtStageContext ctx) =>
			IsInBlockLeftBound(ctx) || IsInBlockRightBound(ctx);

		private bool IsInBlockRightBound([NotNull] CSharpFmtStageContext ctx)
		{
			if (!(ctx.LeftChild is ICommentNode comment)) return false;
			if (comment.GetText() != StartComment) return false;
			var rightExpressionBlock = ctx.RightChild.GetFirstTokenIn().GetT4ContainerFromCSharpNode<IT4CodeBlock>();
			if (rightExpressionBlock == null) return false;
			return true;
		}

		private bool IsInBlockLeftBound([NotNull] CSharpFmtStageContext ctx)
		{
			if (!(ctx.RightChild is ICommentNode comment)) return false;
			if (comment.GetText() != EndComment) return false;
			var leftExpressionBlock = ctx.LeftChild.GetLastTokenIn().GetT4ContainerFromCSharpNode<IT4CodeBlock>();
			if (leftExpressionBlock == null) return false;
			return true;
		}

		[NotNull]
		protected abstract string StartComment { get; }

		[NotNull]
		protected abstract string EndComment { get; }
	}
}
