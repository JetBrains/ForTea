using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Format;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public abstract class T4AppendableElementDescriptionBase : T4ElementDescriptionBase
	{
		public abstract void AppendContent(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider
		);

		protected string GetLineDirectiveText(ITreeNode node)
		{
			var sourceFile = node.GetSourceFile().NotNull();
			int line =  (int) sourceFile.Document.GetCoordsByOffset(node.GetTreeStartOffset().Offset).Line;
			return $"#line {line + 1} \"{sourceFile.GetLocation()}\"";
		}
	}
}
