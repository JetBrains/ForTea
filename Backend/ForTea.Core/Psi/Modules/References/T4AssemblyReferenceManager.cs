using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;

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
		/// <param name="assemblyNameOrFile"></param>
		/// <remarks>Does not refresh references, simply add a cookie to the cookies list.</remarks>
		[CanBeNull]
		public IAssemblyCookie TryAddReference([NotNull] string assemblyNameOrFile)
		{
			var cookie = CreateCookie(assemblyNameOrFile);
			if (cookie != null)
				References.Add(assemblyNameOrFile, cookie);

			return cookie;
		}

		/// <summary>Creates a new <see cref="IAssemblyCookie"/> from an assembly full name.</summary>
		/// <param name="assemblyNameOrFile">The assembly full name.</param>
		/// <returns>An instance of <see cref="IAssemblyCookie"/>, or <c>null</c> if none could be created.</returns>
		[CanBeNull]
		private IAssemblyCookie CreateCookie([NotNull] string assemblyNameOrFile)
		{
			assemblyNameOrFile = assemblyNameOrFile.Trim();
			if (assemblyNameOrFile.Length == 0) return null;
			var target = Resolver.FindAssemblyReferenceTarget(assemblyNameOrFile);
			if (target == null) return null;
			return CreateCookieCore(target);
		}

		[CanBeNull]
		private IAssemblyCookie CreateCookieCore([NotNull] AssemblyReferenceTarget target)
		{
			// ResolveManager uses providers, not contexts, to resolve references,
			// so it's safe to provide project's contests
			var path = Resolver.Resolve(target, File.GetProject().NotNull(), ResolveContext);
			return path == null ? null : AssemblyFactory.AddRef(path, "T4", ResolveContext);
		}
	}
}
