using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Cache;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules.References
{
	public interface IT4AssemblyReferenceManager : IDisposable
	{
		[NotNull]
		[ItemNotNull]
		IEnumerable<IModule> AssemblyReferences { get; }

		[NotNull]
		[ItemNotNull]
		IEnumerable<IModule> ProjectReferences { get; }

		[NotNull]
		[ItemNotNull]
		IEnumerable<FileSystemPath> RawReferences { get; }

		void AddBaseReferences();

		/// <returns>Whether a change was made</returns>
		bool ProcessDiff([NotNull] T4DeclaredAssembliesDiff diff);
	}
}
