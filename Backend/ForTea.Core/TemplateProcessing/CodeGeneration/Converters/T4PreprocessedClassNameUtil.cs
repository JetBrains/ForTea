using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public static class T4PreprocessedClassNameUtil
	{
		[NotNull]
		public static string CreateGeneratedClassName([NotNull] this IT4File thіs)
		{
			string fileName = thіs.LogicalPsiSourceFile.Name.WithoutExtension();
			if (ValidityChecker.IsValidIdentifier(fileName)) return fileName;
			return T4CSharpIntermediateConverterBase.GeneratedClassNameString;
		}
	}
}
