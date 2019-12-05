using System.Collections.Generic;
using JetBrains.Annotations;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Assemblies.Impl
{
	public sealed class T4LightWeightAssemblyResolutionRequest
	{
		[NotNull, ItemNotNull]
		public IEnumerable<string> AssembliesToResolve { get; }

		public T4LightWeightAssemblyResolutionRequest([NotNull, ItemNotNull] IEnumerable<string> assembliesToResolve) =>
			AssembliesToResolve = assembliesToResolve;
	}
}
