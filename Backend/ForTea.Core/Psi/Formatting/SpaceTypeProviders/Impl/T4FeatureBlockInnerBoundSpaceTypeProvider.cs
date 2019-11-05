using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders.Impl
{
	internal sealed class T4FeatureBlockInnerBoundSpaceTypeProvider :
		T4BlockInnerBoundSpaceTypeProviderBase<IT4FeatureBlock>
	{
		protected override SpaceType Type => SpaceType.Vertical;
		protected override string StartComment => T4CSharpCodeBehindIntermediateConverter.CodeCommentStartText;
		protected override string EndComment => T4CSharpCodeBehindIntermediateConverter.CodeCommentEndText;
	}
}
