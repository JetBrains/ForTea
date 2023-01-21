using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ForTea.Core.Parsing
{
  /// <summary>Lexer filtering whitespaces.</summary>
  internal sealed class T4FilteringLexer : FilteringLexer
  {
    public T4FilteringLexer([NotNull] ILexer lexer) : base(lexer)
    {
    }

    protected override bool Skip(TokenNodeType tokenType) => tokenType.IsFiltered;
  }
}