using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Cache.Impl
{
	[SolutionComponent]
	public sealed class T4IndirectIncludeTransitiveClosureSearcher
	{
		[NotNull]
		private IT4PsiFileSelector Selector { get; }

		public T4IndirectIncludeTransitiveClosureSearcher([NotNull] IT4PsiFileSelector selector) => Selector = selector;

		[NotNull, ItemNotNull]
		public IEnumerable<IPsiSourceFile> FindClosure(
			[NotNull] Func<IPsiSourceFile, T4FileDependencyData> provider,
			[NotNull] IPsiSourceFile file
		) => FindAllIncludes(provider, FindAllIncluders(provider, file));

		/// <summary>
		/// Performs DFS to collect all the files that include the current one,
		/// avoiding loops in includes if necessary
		/// </summary>
		[NotNull, ItemNotNull]
		private IEnumerable<IPsiSourceFile> FindAllIncluders(
			[NotNull] Func<IPsiSourceFile, T4FileDependencyData> provider,
			[NotNull] IPsiSourceFile file
		)
		{
			var result = new JetHashSet<IPsiSourceFile>();
			FindAllParents(file, provider, result);
			return result;
		}

		[NotNull, ItemNotNull]
		private IEnumerable<IPsiSourceFile> FindAllIncludes(
			[NotNull] Func<IPsiSourceFile, T4FileDependencyData> provider,
			[NotNull, ItemNotNull] IEnumerable<IPsiSourceFile> includers
		)
		{
			var result = new JetHashSet<IPsiSourceFile>();
			foreach (var includer in includers)
			{
				FindAllChildren(includer, provider, result);
			}

			return result;
		}

		private void FindAllChildren(
			[NotNull] IPsiSourceFile file,
			[NotNull] Func<IPsiSourceFile, T4FileDependencyData> provider,
			[NotNull, ItemNotNull] ISet<IPsiSourceFile> destination
		)
		{
			if (destination.Contains(file)) return;
			destination.Add(file);
			var data = provider(file);
			if (data == null) return;
			foreach (var child in data.Includes)
			{
				FindAllChildren(Selector.FindMostSuitableFile(child, file), provider, destination);
			}
		}

		private void FindAllParents(
			[NotNull] IPsiSourceFile file,
			[NotNull] Func<IPsiSourceFile, T4FileDependencyData> provider,
			[NotNull, ItemNotNull] ISet<IPsiSourceFile> destination
		)
		{
			if (destination.Contains(file)) return;
			destination.Add(file);
			var data = provider(file);
			if (data == null) return;
			foreach (var parent in data.Includers)
			{
				FindAllParents(Selector.FindMostSuitableFile(parent, file), provider, destination);
			}
		}
	}
}
