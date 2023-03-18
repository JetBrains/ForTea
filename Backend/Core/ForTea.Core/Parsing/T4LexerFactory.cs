using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace GammaJul.ForTea.Core.Parsing
{
  /// <summary>Factory creating <see cref="T4Lexer"/>.</summary>
  internal sealed class T4LexerFactory : ILexerFactory
  {
    [NotNull] public static T4LexerFactory Instance { get; } = new T4LexerFactory();

    private T4LexerFactory()
    {
    }

    public ILexer CreateLexer(IBuffer buffer) => new T4Lexer(buffer);
  }
}