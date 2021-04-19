using GammaJul.ForTea.Core.Parsing.Parser.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.Tests.Mock
{
	public sealed class T4MockPsiFileProvider : IT4PsiFileProvider
	{
		[NotNull]
		private IFile File { get; }

		public T4MockPsiFileProvider([NotNull] IFile file) => File = file;
		public IT4File GetPsi() => (IT4File) File;
	}
}
