using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Components;
using JetBrains.Application.Parts;
using JetBrains.Application.RdFramework.Reflection;
using JetBrains.Application.Threading;
using JetBrains.DocumentModel;
using JetBrains.ForTea.ReSharperPlugin.AnyEnd;
using JetBrains.Interop.WinApi;
using JetBrains.Lifetimes;
using JetBrains.Platform.RdFramework.RdVerification;
using JetBrains.Platform.RdFramework;
using JetBrains.ProjectModel.Search;
using JetBrains.ProjectModel;
using JetBrains.Rd.Reflection;
using JetBrains.VsIntegration.Interop;
using JetBrains.VsIntegration.ProjectDocuments.Projects.Builder;
using JetBrains.VsIntegration.ProjectModel;
using JetBrains.VsIntegration.Util;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.Collections.Generic;

namespace JetBrains.ForTea.ReSharperPlugin.Frontend;

[RdExt]
[ZoneMarker(typeof(IDocumentModelZone), typeof(IProjectModelZone), typeof(IRdFrameworkZone))]
[SolutionComponent(Instantiation.DemandAnyThreadSafe)]
public sealed class T4VisualStudioInterop : MultiThreadInitRdExtReflectionBindableBase, IT4VisualStudioInterop
{
  private readonly IShellLocks myLocks;
  private readonly ILazy<ProjectModelSynchronizer> mySynchronizer;
  private readonly RawVsServiceProvider myProvider;

  public T4VisualStudioInterop(
    Lifetime lifetime, IRdShell rd, IShellLocks locks,
    ILazy<ProjectModelSynchronizer> synchronizer, RawVsServiceProvider provider)
    : base(lifetime, rd)
  {
    myLocks = locks;
    mySynchronizer = synchronizer;
    myProvider = provider;
  }

  public Dictionary<string, string> ResolveMacros(IProjectModelElementSearchDescriptor project, string[] macros)
  {
    myLocks.AssertMainThread();
    using var cookie = myLocks.UsingReadLock();
    var hierarchy = T4ResolutionUtils.TryGetVsHierarchy(mySynchronizer.Value, project);
    var macroInfo = hierarchy as IVsBuildMacroInfo;
    var result = new Dictionary<string, string>();
    foreach (var macro in macros)
    {
      string value = null;
      bool succeeded = macroInfo != null
                       && HResultHelpers.SUCCEEDED(macroInfo.GetBuildMacroValue(macro, out value))
                       && !string.IsNullOrEmpty(value);
      if (!succeeded)
        value = MSBuildExtensions.GetStringValue(hierarchy, macro, null);
      if (!string.IsNullOrEmpty(value))
        result[macro] = value;
    }
    return result;
  }

  public Dictionary<string, string> ResolveAssemblies(
    IProjectModelElementSearchDescriptor project, string inputFile, string[] assemblyReferences)
  {
    myLocks.AssertMainThread();
    using var cookie = myLocks.UsingReadLock();
    var result = new Dictionary<string, string>();
    var components = myProvider.Value.GetService<STextTemplating, ITextTemplatingComponents>();
    if (components == null) return result;

    var oldHierarchy = components.Hierarchy;
    var oldInputFile = components.InputFile;
    try
    {
      components.Hierarchy = T4ResolutionUtils.TryGetVsHierarchy(mySynchronizer.Value, project);
      components.InputFile = inputFile;
      foreach (var reference in assemblyReferences)
      {
        var resolved = components.Host?.ResolveAssemblyReference(reference);
        if (!string.IsNullOrEmpty(resolved))
          result[reference] = resolved;
      }
    }
    finally
    {
      components.Hierarchy = oldHierarchy;
      components.InputFile = oldInputFile;
    }
    return result;
  }
}
