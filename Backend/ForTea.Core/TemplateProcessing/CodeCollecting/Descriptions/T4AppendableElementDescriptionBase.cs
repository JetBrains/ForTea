using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public abstract class T4AppendableElementDescriptionBase : T4ElementDescriptionBase
	{
		public abstract void AppendContent(
			[NotNull] T4CSharpCodeGenerationResult destination,
			[NotNull] IT4ElementAppendFormatProvider provider,
			[NotNull] IT4File context);

		protected string GetLineDirectiveText(ITreeNode node)
		{
			var sourceFile = node.GetSourceFile().NotNull();
			int line =  (int) sourceFile.Document.GetCoordsByOffset(node.GetTreeStartOffset().Offset).Line;
			return $"#line {line + 1} \"{sourceFile.GetLocation()}\"";
		}

		protected static Int32<DocColumn> GetOffset(ITreeNode node) =>
			node.GetSourceFile().NotNull().Document.GetCoordsByOffset(node.GetTreeStartOffset().Offset).Column;

		protected T4AppendableElementDescriptionBase([CanBeNull] IT4File source = null) : base(source)
		{
		}
	}
}
