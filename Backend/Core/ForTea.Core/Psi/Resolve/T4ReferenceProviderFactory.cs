using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
  [ReferenceProviderFactory]
  public sealed class T4ReferenceProviderFactory : IReferenceProviderFactory
  {
    public ISignal<IReferenceProviderFactory> Changed { get; }

    [NotNull] private IT4IncludeResolver IncludeResolver { get; }

    // ReSharper disable once AssignNullToNotNullAttribute
    public T4ReferenceProviderFactory(Lifetime lifetime, [NotNull] IT4IncludeResolver includeResolver)
    {
      IncludeResolver = includeResolver;
      Changed = new Signal<IReferenceProviderFactory>(lifetime, GetType().FullName);
    }

    public IReferenceFactory CreateFactory(IPsiSourceFile sourceFile, IFile file, IWordIndex wordIndexForChecks) =>
      sourceFile.PrimaryPsiLanguage.Is<T4Language>() ? new T4ReferenceFactory(IncludeResolver) : null;
  }
}