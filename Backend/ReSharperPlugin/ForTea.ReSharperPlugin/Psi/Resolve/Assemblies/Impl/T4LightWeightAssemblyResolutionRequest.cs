using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Assemblies.Impl
{
	public sealed class T4LightWeightAssemblyResolutionRequest
	{
		[NotNull, ItemNotNull]
		public IEnumerable<T4ResolvedPath> AssembliesToResolve { get; }

		public T4LightWeightAssemblyResolutionRequest([NotNull, ItemNotNull] IEnumerable<T4ResolvedPath> assembliesToResolve) =>
			AssembliesToResolve = assembliesToResolve;
	}
}
