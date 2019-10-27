using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Parsing.Ranges;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	internal partial class IncludedFile
	{
		public IDocumentRangeTranslator DocumentRangeTranslator { get; private set; }
		public IPsiSourceFile LogicalPsiSourceFile { get; private set; }
		public IPsiSourceFile PhysicalPsiSourceFile => GetSourceFile();
		public IEnumerable<IT4IncludedFile> Includes => this.Children<IT4IncludedFile>();

		[NotNull]
		public static IncludedFile FromOtherNode([NotNull] IT4FileLikeNode node)
		{
			var includedFile = (IncludedFile) TreeElementFactory.CreateCompositeElement(ElementType.INCLUDED_FILE);
			foreach (var child in node.Children().Cast<TreeElement>().ToList())
			{
				includedFile.AppendNewChild(child);
			}

			includedFile.LogicalPsiSourceFile = node.LogicalPsiSourceFile;
			includedFile.DocumentRangeTranslator = new T4DocumentRangeTranslator(includedFile);
			return includedFile;
		}
	}
}
