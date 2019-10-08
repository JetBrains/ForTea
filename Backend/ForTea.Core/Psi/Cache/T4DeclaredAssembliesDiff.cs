using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	/// <summary>Represents the difference between two <see cref="T4DeclaredAssembliesInfo" />.</summary>
	public sealed class T4DeclaredAssembliesDiff
	{
		/// <summary>Gets an enumeration of all added assemblies.</summary>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<IT4PathWithMacros> AddedAssemblies { get; }

		/// <summary>Gets an enumeration of all removed assemblies.</summary>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<IT4PathWithMacros> RemovedAssemblies { get; }

		public T4DeclaredAssembliesDiff(
			[NotNull] [ItemNotNull] IEnumerable<IT4PathWithMacros> addedAssemblies,
			[NotNull] [ItemNotNull] IEnumerable<IT4PathWithMacros> removedAssemblies
		)
		{
			AddedAssemblies = addedAssemblies;
			RemovedAssemblies = removedAssemblies;
		}
	}
}
