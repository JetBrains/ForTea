using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Macros
{
	public sealed class T4MacroResolutionRequest
	{
		[NotNull, ItemNotNull]
		public IList<IT4Macro> MacrosToResolve { get; }

		public T4MacroResolutionRequest([NotNull, ItemNotNull] IList<IT4Macro> macrosToResolve) =>
			MacrosToResolve = macrosToResolve;
	}
}
