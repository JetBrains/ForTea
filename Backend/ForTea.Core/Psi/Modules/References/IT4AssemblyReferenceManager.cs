using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
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

		[NotNull]
		IModuleReferenceResolveContext ResolveContext { get; }

		/// <returns>Whether a change was made</returns>
		bool TryRemoveReference([NotNull] IT4PathWithMacros pathWithMacros);

		/// <summary>Try to add an assembly reference to the list of assemblies.</summary>
		/// <note> Does not refresh references, simply add a cookie to the cookies list. </note>
		/// <returns>Whether a change was made</returns>
		bool TryAddReference([NotNull] IT4PathWithMacros pathWithMacros);
	}
}
