using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;

namespace GammaJul.ForTea.Core.Parsing.Parser.Impl
{
	public sealed class T4PsiFromSourceProvider : IT4PsiFileProvider
	{
		[NotNull]
		private IPsiSourceFile PsiSourceFile { get; }

		public T4PsiFromSourceProvider([NotNull] IPsiSourceFile psiSourceFile) => PsiSourceFile = psiSourceFile;

		public IT4File GetPsi()
		{
			var primaryPsiFile = PsiSourceFile.GetPrimaryPsiFile();
			if (primaryPsiFile is not IT4File t4File)
			{
				throw new InvalidOperationException("The root file has to be a T4 file");
			}

			return t4File;
		}
	}
}
