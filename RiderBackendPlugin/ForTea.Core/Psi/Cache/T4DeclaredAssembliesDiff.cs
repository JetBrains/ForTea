using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	/// <summary>Represents the difference between two <see cref="T4DeclaredAssembliesInfo" />.</summary>
	public sealed class T4DeclaredAssembliesDiff
	{
		/// <summary>Gets an enumeration of all added assemblies.</summary>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<T4ResolvedPath> AddedAssemblies { get; }

		/// <summary>Gets an enumeration of all removed assemblies.</summary>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<T4ResolvedPath> RemovedAssemblies { get; }

		public T4DeclaredAssembliesDiff(
			[NotNull] [ItemNotNull] IEnumerable<T4ResolvedPath> addedAssemblies,
			[NotNull] [ItemNotNull] IEnumerable<T4ResolvedPath> removedAssemblies
		)
		{
			AddedAssemblies = addedAssemblies;
			RemovedAssemblies = removedAssemblies;
		}
	}
}
