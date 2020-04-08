using JetBrains.Annotations;
using JetBrains.Application.changes;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	internal sealed class T4WriteOnlyChangeProvider : IChangeProvider
	{
		[CanBeNull]
		public object Execute(IChangeMap changeMap) => null;
	}
}
