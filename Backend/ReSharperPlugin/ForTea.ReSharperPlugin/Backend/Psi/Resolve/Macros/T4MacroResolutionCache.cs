using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Application.Threading;
using JetBrains.DataFlow;
using JetBrains.Diagnostics;
using JetBrains.ForTea.ReSharperPlugin.AnyEnd;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.Util;

namespace JetBrains.ForTea.ReSharperPlugin.Backend.Psi.Resolve.Macros
{
  /// <summary>
  /// In R#, it is only possible to resolve macros on the main thread,
  /// so we have to cache them to be able to access them from the daemon
  /// </summary>
  [PsiComponent(InstantiationEx.LegacyDefault)]
  public sealed class T4MacroResolutionCache : T4PsiAwareCacheBase<T4MacroResolutionRequest, T4MacroResolutionData>
  {
    private readonly IT4VisualStudioInterop myVisualStudio;

    [NotNull] public Signal<IPsiSourceFile> OnFileMarkedForInvalidation { get; }

    public T4MacroResolutionCache(
      Lifetime lifetime,
      [NotNull] IShellLocks locks,
      [NotNull] IPersistentIndexManager persistentIndexManager,
      [NotNull] IT4VisualStudioInterop visualStudio
    ) : base(lifetime, locks, persistentIndexManager, T4MacroResolutionDataMarshaller.Instance)
    {
      myVisualStudio = visualStudio;
      OnFileMarkedForInvalidation = new(lifetime, "Update from T4MacroResolutionCache");
    }

    [NotNull]
    protected override T4MacroResolutionRequest Build(IT4File file)
    {
      var macros = file
        .GetThisAndChildrenOfType<IT4Macro>()
        .Where(macro => macro.RawAttributeValue != null)
        .Distinct(macro => macro.RawAttributeValue.GetText());
      return new T4MacroResolutionRequest(macros.ToList());
    }

    public override void Merge(IPsiSourceFile sourceFile, object builtPart)
    {
      var request = (T4MacroResolutionRequest)builtPart.NotNull();
      var projectFile = sourceFile.ToProjectFile().NotNull();
      var macroNames = request.MacrosToResolve.Select(macro => macro.RawAttributeValue.GetText()).ToArray();
      var oldKeys = Map.TryGetValue(sourceFile)?.ResolvedMacros.Keys ?? EmptyList<string>.Instance;
      var resolved = macroNames.Length == 0
        ? new Dictionary<string, string>()
        : myVisualStudio.ResolveMacros(projectFile.GetProject().NotNull().ToProjectSearchDescriptor(), macroNames);
      var data = new T4MacroResolutionData(resolved);
      var newKeys = data.ResolvedMacros.Keys;
      base.Merge(sourceFile, data);

      if (!newKeys.All(it => oldKeys.Contains(it)))
      {
        OnFileMarkedForInvalidation.Fire(sourceFile);
      }
    }

    [CanBeNull]
    public T4MacroResolutionData TryGetValue(IPsiSourceFile sourceFile)
    {
      base.TryGetValue(sourceFile, out var value);
      return value;
    }
  }
}
