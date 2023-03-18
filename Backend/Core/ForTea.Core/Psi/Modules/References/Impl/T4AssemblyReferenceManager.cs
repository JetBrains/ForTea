using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules.References.Impl
{
  public sealed class T4AssemblyReferenceManager : IT4AssemblyReferenceManager
  {
    [NotNull] private IT4AssemblyReferenceResolver AssemblyReferenceResolver { get; }

    [NotNull] private IT4ProjectReferenceResolver ProjectReferenceResolver { get; }

    [NotNull] private IShellLocks ShellLocks { get; }

    [NotNull] private IT4Environment Environment { get; }

    [NotNull] private IDictionary<VirtualFileSystemPath, IAssemblyCookie> MyAssemblyReferences { get; }

    private IDictionary<VirtualFileSystemPath, IProject> MyProjectReferences { get; }

    public IEnumerable<IModule> AssemblyReferences =>
      MyAssemblyReferences.Values.SelectNotNull(cookie => cookie.Assembly);

    public IEnumerable<IModule> ProjectReferences => MyProjectReferences.Values;

    [NotNull] private IPsiSourceFile SourceFile { get; }

    [NotNull] private IProjectFile ProjectFile { get; }

    [NotNull] private ISolution Solution { get; }

    [NotNull] private IAssemblyFactory AssemblyFactory { get; }

    private IModuleReferenceResolveContext ResolveContext { get; }

    internal T4AssemblyReferenceManager(
      [NotNull] IAssemblyFactory assemblyFactory,
      [NotNull] IPsiSourceFile sourceFile,
      [NotNull] IProjectFile projectFile,
      [NotNull] IModuleReferenceResolveContext resolveContext,
      [NotNull] IShellLocks shellLocks
    )
    {
      MyProjectReferences = new Dictionary<VirtualFileSystemPath, IProject>();
      MyAssemblyReferences = new Dictionary<VirtualFileSystemPath, IAssemblyCookie>();
      AssemblyFactory = assemblyFactory;
      ProjectFile = projectFile;
      ResolveContext = resolveContext;
      ShellLocks = shellLocks;
      SourceFile = sourceFile;
      Solution = ProjectFile.GetSolution();
      AssemblyReferenceResolver = Solution.GetComponent<IT4AssemblyReferenceResolver>();
      ProjectReferenceResolver = Solution.GetComponent<IT4ProjectReferenceResolver>();
      Environment = Solution.GetComponent<IT4Environment>();
    }

    private bool TryRemoveReference([NotNull] T4ResolvedPath pathWithMacros)
    {
      var path = AssemblyReferenceResolver.ResolveWithoutCaching(pathWithMacros);
      if (path == null) return false;
      if (MyAssemblyReferences.TryGetValue(path, out var cookie))
      {
        MyAssemblyReferences.Remove(path);
        cookie.Dispose();
        return true;
      }

      if (MyProjectReferences.ContainsKey(path))
      {
        MyProjectReferences.Remove(path);
        return true;
      }

      return false;
    }

    private bool TryAddReference([NotNull] T4ResolvedPath pathWithMacros)
    {
      var path = AssemblyReferenceResolver.ResolveWithoutCaching(pathWithMacros);
      if (path == null) return false;
      if (MyAssemblyReferences.ContainsKey(path)) return false;
      if (MyProjectReferences.ContainsKey(path)) return false;
      return TryAddProjectReference(path) || TryAddAssemblyReference(path);
    }

    private bool TryAddProjectReference([NotNull] VirtualFileSystemPath path)
    {
      var project = ProjectReferenceResolver.TryResolveProject(path);
      if (project == null) return false;
      MyProjectReferences.Add(path, project);
      return true;
    }

    private bool TryAddAssemblyReference([NotNull] VirtualFileSystemPath path)
    {
      var cookie = AssemblyFactory.AddRef(new AssemblyLocation(path), "T4", ResolveContext);
      if (cookie == null) return false;
      MyAssemblyReferences.Add(path, cookie);
      return true;
    }

    public void AddBaseReferences()
    {
      foreach (string assemblyName in Environment.DefaultAssemblyNames)
      {
        TryAddReference(assemblyName);
      }
    }

    private void TryAddReference([NotNull] string name) =>
      TryAddReference(new T4ResolvedPath(name, SourceFile, ProjectFile));

    public bool ProcessDiff(T4DeclaredAssembliesDiff diff)
    {
      bool hasChanges = false;
      // removes the assembly references from the old assembly directives
      foreach (var _ in diff.RemovedAssemblies.Where(TryRemoveReference))
      {
        hasChanges = true;
      }

      // adds assembly references from the new assembly directives
      foreach (var _ in diff.AddedAssemblies.Where(TryAddReference))
      {
        hasChanges = true;
      }

      return hasChanges;
    }

    public void Dispose()
    {
      var assemblyCookies = MyAssemblyReferences.Values.ToArray();
      if (assemblyCookies.Length == 0) return;
      ShellLocks.ExecuteWithWriteLock(
        () =>
        {
          foreach (var assemblyCookie in assemblyCookies)
          {
            assemblyCookie.Dispose();
          }
        }
      );
      MyAssemblyReferences.Clear();
    }
  }
}