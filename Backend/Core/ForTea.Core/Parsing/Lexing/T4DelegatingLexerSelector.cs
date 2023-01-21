using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;

namespace GammaJul.ForTea.Core.Parsing.Lexing
{
  public sealed class T4DelegatingLexerSelector : IT4LexerSelector
  {
    [NotNull] private ILexer CustomLexer { get; }

    [NotNull] private IPsiSourceFile SourceFileForCustomLexer { get; }

    [NotNull] private IT4LexerSelector Delegate { get; }

    public T4DelegatingLexerSelector(
      [NotNull] ILexer customLexer,
      [NotNull] IPsiSourceFile sourceFileForCustomLexer,
      [NotNull] IT4LexerSelector @delegate
    )
    {
      CustomLexer = customLexer;
      SourceFileForCustomLexer = sourceFileForCustomLexer;
      Delegate = @delegate;
    }

    public ILexer SelectLexer(IPsiSourceFile file)
    {
      if (file == SourceFileForCustomLexer) return CustomLexer;
      return Delegate.SelectLexer(file);
    }
  }
}