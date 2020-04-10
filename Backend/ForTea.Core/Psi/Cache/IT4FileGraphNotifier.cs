using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	public interface IT4FileGraphNotifier
	{
		event Action<IEnumerable<IPsiSourceFile>> OnFilesIndirectlyAffected;
	}
}
