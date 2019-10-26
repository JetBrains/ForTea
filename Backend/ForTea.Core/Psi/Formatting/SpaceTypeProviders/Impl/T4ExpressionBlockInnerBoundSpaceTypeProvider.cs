using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders.Impl
{
	internal sealed class T4ExpressionBlockInnerBoundSpaceTypeProvider :
		T4BlockInnerBoundSpaceTypeProviderBase<IT4ExpressionBlock>
	{
		protected override SpaceType Type => SpaceType.NoSpace;
		protected override string StartComment => T4CSharpCodeBehindIntermediateConverter.ExpressionCommentStartText;
		protected override string EndComment => T4CSharpCodeBehindIntermediateConverter.ExpressionCommentEndText;
	}
}
