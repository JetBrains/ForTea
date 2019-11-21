using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules.References
{
	public sealed class T4AssemblyReferenceManager : IDisposable
	{
		[NotNull]
		private IT4AssemblyReferenceResolver Resolver { get; }

		[NotNull]
		private IShellLocks ShellLocks { get; }

		[NotNull]
		private Dictionary<FileSystemPath, IAssemblyCookie> References { get; } =
			new Dictionary<FileSystemPath, IAssemblyCookie>();

		[NotNull, ItemNotNull]
		public IEnumerable<IAssembly> RawReferences => References.Values.SelectNotNull(cookie => cookie.Assembly);

		[NotNull]
		private IProjectFile File { get; }

		[NotNull]
		private IAssemblyFactory AssemblyFactory { get; }

		public IModuleReferenceResolveContext ResolveContext { get; }

		internal T4AssemblyReferenceManager(
			[NotNull] IAssemblyFactory assemblyFactory,
			[NotNull] IProjectFile file,
			[NotNull] IModuleReferenceResolveContext resolveContext,
			[NotNull] IShellLocks shellLocks
		)
		{
			AssemblyFactory = assemblyFactory;
			File = file;
			ResolveContext = resolveContext;
			ShellLocks = shellLocks;
			Resolver = File.GetSolution().GetComponent<IT4AssemblyReferenceResolver>();
		}

		/// <returns>Whether a change was made</returns>
		public bool TryRemoveReference([NotNull] IT4PathWithMacros pathWithMacros)
		{
			var path = Resolver.Resolve(pathWithMacros);
			if (path == null) return false;
			if (!References.TryGetValue(path, out var cookie)) return false;
			References.Remove(path);
			cookie.Dispose();
			return true;
		}

		/// <summary>Try to add an assembly reference to the list of assemblies.</summary>
		/// <note> Does not refresh references, simply add a cookie to the cookies list. </note>
		[CanBeNull]
		public IAssemblyCookie TryAddReference([NotNull] IT4PathWithMacros pathWithMacros)
		{
			var path = Resolver.Resolve(pathWithMacros);
			if (path == null) return null;
			var cookie = AssemblyFactory.AddRef(path, "T4", ResolveContext);
			if (cookie != null) References.Add(path, cookie);
			return cookie;
		}

		public void Dispose()
		{
			var assemblyCookies = References.Values.ToArray();
			if (assemblyCookies.Length == 0) return;
			ShellLocks.ExecuteWithWriteLock(
				() =>
				{
					foreach (var assemblyCookie in assemblyCookies)
					{
						assemblyCookie.Dispose();
					}
				}
			);
			References.Clear();
		}
	}
}
