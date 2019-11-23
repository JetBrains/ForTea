using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	public interface IT4FilePsiModule : IProjectPsiModule
	{
		[NotNull]
		IPsiSourceFile SourceFile { get; }

		[NotNull, ItemNotNull]
		IEnumerable<FileSystemPath> RawReferences { get; }
	}
}
