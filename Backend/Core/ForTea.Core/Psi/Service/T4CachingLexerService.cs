using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CachingLexers;

namespace GammaJul.ForTea.Core.Psi.Service
{
  [Language(typeof(T4Language))]
  public class T4CachingLexerService : MixedCachingLexerService
  {
  }
}