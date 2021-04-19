using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Parsing.Parser.Impl
{
	public interface IT4PsiFileProvider
	{
		[NotNull]
		IT4File GetPsi();
	}
}
