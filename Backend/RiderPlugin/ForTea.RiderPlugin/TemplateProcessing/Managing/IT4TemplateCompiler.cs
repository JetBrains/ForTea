using JetBrains.Annotations;
using JetBrains.ForTea.RiderPlugin.Model;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
  public interface IT4TemplateCompiler
  {
    T4BuildResult Compile(Lifetime lifetime, [NotNull] IPsiSourceFile file);
  }
}