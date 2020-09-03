using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4FeatureExpressionDescription : T4ExpressionDescription
	{
		public T4FeatureExpressionDescription([NotNull] IT4Code source) : base(source)
		{
		}

		protected override void AppendContentPrefix(
			T4CSharpCodeGenerationResult destination,
			IT4ElementAppendFormatProvider provider
		)
		{
			destination.Append(provider.ExpressionWritingPrefix);
			destination.Append(provider.ToStringConversionPrefix);
		}
	}
}
