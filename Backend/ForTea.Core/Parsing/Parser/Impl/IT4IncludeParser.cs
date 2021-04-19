using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing.Parser.Impl
{
	public interface IT4IncludeParser
	{
		[CanBeNull]
		ITreeNode Parse([NotNull] IT4IncludeDirective directive);
	}
}
