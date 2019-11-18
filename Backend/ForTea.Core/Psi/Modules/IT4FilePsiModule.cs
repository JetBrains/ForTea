using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	public interface IT4FilePsiModule : IProjectPsiModule
	{
		[NotNull]
		IPsiSourceFile SourceFile { get; }

		[NotNull, ItemNotNull]
		IEnumerable<IAssembly> RawReferences { get; }
	}
}
