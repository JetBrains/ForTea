using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl
{
  [SolutionComponent]
  public sealed class T4IncludeResolver : IT4IncludeResolver
  {
    [NotNull] private IT4PsiFileSelector Selector { get; }

    [NotNull] private IT4Environment Environment { get; }

    public T4IncludeResolver([NotNull] IT4PsiFileSelector selector, [NotNull] IT4Environment environment)
    {
      Selector = selector;
      Environment = environment;
    }

    public VirtualFileSystemPath ResolvePath(T4ResolvedPath path)
    {
      var absolute = path.TryResolveAbsolutePath();
      if (absolute != null) return absolute;

      // search in global include paths
      var asGlobalInclude = Environment.IncludePaths
        .Select(includePath => includePath.Combine(path.ResolvedPath))
        .FirstOrDefault(resultPath => resultPath.ExistsFile);

      return asGlobalInclude ?? VirtualFileSystemPath.GetEmptyPathFor(InteractionContext.SolutionContext);
    }

    public IPsiSourceFile Resolve(T4ResolvedPath path) =>
      Selector.FindMostSuitableFile(ResolvePath(path), path.SourceFile);
  }
}