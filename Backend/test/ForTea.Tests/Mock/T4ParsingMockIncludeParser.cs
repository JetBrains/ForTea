using GammaJul.ForTea.Core.Parsing.Parser.Impl;
using GammaJul.ForTea.Core.Test;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.Tests.Mock
{
	public sealed class T4ParsingMockIncludeParser : IT4IncludeParser
	{
		[NotNull]
		private string Text { get; }

		public T4ParsingMockIncludeParser([NotNull] string text) => Text = text;

		public ITreeNode Parse(IT4IncludeDirective directive) =>
			T4ParserExposer.Create(Text, new T4MockIncludeParser()).ParseFileWithoutCleanup();
	}
}
