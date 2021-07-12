using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;

namespace GammaJul.ForTea.Core.Psi.Formatting.SpaceTypeProviders.Impl
{
	[ProjectFileType(typeof(T4ProjectFileType))]
	internal sealed class T4ExpressionBlockInnerBoundSpaceTypeProvider : T4BlockInnerBoundSpaceTypeProviderBase
	{
		protected override SpaceType Type => SpaceType.Horizontal;
		protected override string StartComment => T4CSharpCodeBehindIntermediateConverter.ExpressionCommentStartText;
		protected override string EndComment => T4CSharpCodeBehindIntermediateConverter.ExpressionCommentEndText;
	}
}
