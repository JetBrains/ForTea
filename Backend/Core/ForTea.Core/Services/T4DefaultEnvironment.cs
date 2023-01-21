using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Services
{
  [ShellComponent]
  public class T4DefaultEnvironment : IT4Environment
  {
    public virtual TargetFrameworkId TargetFrameworkId { get; } = TargetFrameworkId.Default;
    public virtual CSharpLanguageLevel CSharpLanguageLevel { get; } = CSharpLanguageLevel.Latest;

    public virtual IEnumerable<string> DefaultAssemblyNames
    {
      get
      {
        yield return "mscorlib";
        yield return "System";
        yield return "System.Core";
        yield return "System.Data";
        yield return "System.Xml";
      }
    }

    public virtual IEnumerable<VirtualFileSystemPath> AdditionalCompilationAssemblyLocations =>
      EmptyList<VirtualFileSystemPath>.Enumerable;

    public virtual bool IsSupported => false;
    public virtual IEnumerable<VirtualFileSystemPath> IncludePaths => EmptyList<VirtualFileSystemPath>.Enumerable;
  }
}