using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;

namespace GammaJul.ForTea.Core.Psi.Modules.References
{
	public sealed class T4AssemblyReferenceManager
	{
		[NotNull]
		private IT4AssemblyReferenceResolver Resolver { get; }

		[NotNull]
		public Dictionary<string, IAssemblyCookie> References { get; } =
			new Dictionary<string, IAssemblyCookie>(StringComparer.OrdinalIgnoreCase);

		[NotNull]
		private IProjectFile File { get; }

		[NotNull]
		private IAssemblyFactory AssemblyFactory { get; }

		public IModuleReferenceResolveContext ResolveContext { get; }

		internal T4AssemblyReferenceManager(
			[NotNull] IAssemblyFactory assemblyFactory,
			[NotNull] IProjectFile file,
			[NotNull] IModuleReferenceResolveContext resolveContext
		)
		{
			AssemblyFactory = assemblyFactory;
			File = file;
			ResolveContext = resolveContext;
			Resolver = File.GetSolution().GetComponent<IT4AssemblyReferenceResolver>();
		}

		/// <summary>Try to add an assembly reference to the list of assemblies.</summary>
		/// <note> Does not refresh references, simply add a cookie to the cookies list. </note>
		[CanBeNull]
		public IAssemblyCookie TryAddReference([NotNull] IT4PathWithMacros pathWithMacros)
		{
			var path = Resolver.Resolve(pathWithMacros);
			if (path == null) return null;
			var cookie = AssemblyFactory.AddRef(path, "T4", ResolveContext);
			if (cookie != null) References.Add(pathWithMacros.RawPath, cookie);
			return cookie;
		}
	}
}
