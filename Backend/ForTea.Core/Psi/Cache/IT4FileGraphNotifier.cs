using System.Collections.Generic;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	public interface IT4FileGraphNotifier
	{
		Signal<IEnumerable<IPsiSourceFile>> OnFilesIndirectlyAffected { get; }
	}
}
