using GammaJul.ForTea.Core.Parsing.Parser.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.Tests.Mock
{
	public sealed class T4MockIncludeParser : IT4IncludeParser

	{
		public ITreeNode Parse(IT4IncludeDirective directive) => null;
	}
}
