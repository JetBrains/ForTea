using System;
using System.Collections.Generic;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	public interface IT4FileGraphNotifier
	{
		event Action<IEnumerable<FileSystemPath>> OnFilesIndirectlyAffected;
	}
}
