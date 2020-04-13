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
		[NotNull]
		private IT4PsiFileSelector Selector { get; }

		[NotNull]
		private IT4Environment Environment { get; }

		public T4IncludeResolver([NotNull] IT4PsiFileSelector selector, [NotNull] IT4Environment environment)
		{
			Selector = selector;
			Environment = environment;
		}

		public FileSystemPath ResolvePath(IT4PathWithMacros path)
		{
			var absolute = path.TryResolveAbsolutePath();
			if (absolute != null) return absolute;

			string expanded = path.ResolveString();
			// search in global include paths
			var asGlobalInclude = Environment.IncludePaths
				.Select(includePath => includePath.Combine(expanded))
				.FirstOrDefault(resultPath => resultPath.ExistsFile);

			return asGlobalInclude ?? FileSystemPath.Empty;
		}

		public IPsiSourceFile Resolve(IT4PathWithMacros path) =>
			Selector.FindMostSuitableFile(ResolvePath(path), path.SourceFile);
	}
}
