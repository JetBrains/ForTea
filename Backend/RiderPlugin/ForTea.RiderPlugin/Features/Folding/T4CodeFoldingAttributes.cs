using JetBrains.TextControl.DocumentMarkup;

namespace JetBrains.ForTea.RiderPlugin.Features.Folding
{
	[RegisterHighlighter(Directive, EffectType = EffectType.FOLDING)]
	[RegisterHighlighter(ExpressionBlock, EffectType = EffectType.FOLDING)]
	[RegisterHighlighter(FeatureBlock, EffectType = EffectType.FOLDING)]
	[RegisterHighlighter(StatementBlock, EffectType = EffectType.FOLDING)]
	public static class T4CodeFoldingAttributes
	{
		public const string Directive = "T4 Directive Folding";
		public const string ExpressionBlock = "T4 Expression Block Folding";
		public const string FeatureBlock = "T4 Feature Block Folding";
		public const string StatementBlock = "T4 Statement Block Folding";
	}
}
