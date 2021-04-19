using GammaJul.ForTea.Core.Parsing.Parser.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.Tests.Mock
{
	public sealed class T4MockIncludeParser : IT4IncludeParser
	{
		[CanBeNull]
		private ITreeNode Node { get; }
		public T4MockIncludeParser([CanBeNull] ITreeNode node = null) => Node = node;
		public ITreeNode Parse(IT4IncludeDirective directive) => Node;
	}
}
