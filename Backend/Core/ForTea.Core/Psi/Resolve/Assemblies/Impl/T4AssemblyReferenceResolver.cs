using System;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl
{
	[SolutionComponent]
	public class T4AssemblyReferenceResolver : IT4AssemblyReferenceResolver
	{
		[NotNull]
		private IT4LightWeightAssemblyReferenceResolver LightWeightResolver { get; }

		[NotNull]
		private IModuleReferenceResolveManager ResolveManager { get; }

		public T4AssemblyReferenceResolver(
			[NotNull] IModuleReferenceResolveManager resolveManager,
			[NotNull] IT4LightWeightAssemblyReferenceResolver lightWeightResolver
		)
		{
			ResolveManager = resolveManager;
			LightWeightResolver = lightWeightResolver;
		}

		[CanBeNull]
		private VirtualFileSystemPath Resolve(
			AssemblyReferenceTarget target,
			IProject project,
			IModuleReferenceResolveContext resolveContext
		) => ResolveManager.Resolve(target, project, resolveContext)?.AssemblyPhysicalPath;

		public VirtualFileSystemPath Resolve(IT4AssemblyDirective directive) => Resolve(directive.ResolvedPath);

		public VirtualFileSystemPath Resolve(string assemblyNameOrFile, IPsiSourceFile sourceFile)
		{
			var projectFile = sourceFile.ToProjectFile();
			if (projectFile == null) return null;
			var path = new T4ResolvedPath(assemblyNameOrFile, sourceFile, projectFile);
			return Resolve(path);
		}

		public virtual VirtualFileSystemPath Resolve(T4ResolvedPath path) =>
			ResolveAsLightReference(path)
			?? ResolveAsAssemblyName(path)
			?? ResolveAsAssemblyFile(path);

		public virtual VirtualFileSystemPath ResolveWithoutCaching(T4ResolvedPath path) => Resolve(path);

		[CanBeNull]
		private VirtualFileSystemPath ResolveAsLightReference([NotNull] T4ResolvedPath pathWithMacros) =>
			LightWeightResolver.TryResolve(pathWithMacros);

		[CanBeNull]
		protected VirtualFileSystemPath ResolveAsAssemblyName([NotNull] T4ResolvedPath pathWithMacros) =>
			ResolveAssemblyNameOrFile(pathWithMacros.ProjectFile, pathWithMacros.ResolvedPath);

		[CanBeNull]
		protected VirtualFileSystemPath ResolveAsAssemblyFile([NotNull] T4ResolvedPath pathWithMacros)
		{
			string name = pathWithMacros.ResolvedPath;
			string nameWithoutExtension = TryRemoveBinaryExtension(name);
			if (nameWithoutExtension == null) return null;
			string fileName = name.Substring(0, name.Length - 4);
			return ResolveAssemblyNameOrFile(pathWithMacros.ProjectFile, fileName);
		}

		[CanBeNull]
		private static string TryRemoveBinaryExtension([NotNull] string name)
		{
			if (name.EndsWith(".dll", StringComparison.Ordinal)) return name.Substring(0, name.Length - 4);
			if (name.EndsWith(".exe", StringComparison.Ordinal)) return name.Substring(0, name.Length - 4);
			return null;
		}

		[CanBeNull]
		private VirtualFileSystemPath ResolveAssemblyNameOrFile(
			[NotNull] IProjectFile projectFile,
			[NotNull] string assemblyNameOrFile
		)
		{
			var resolveContext = projectFile.SelectResolveContext();
			var nameInfo = AssemblyNameInfo.TryParse(assemblyNameOrFile);
			if (nameInfo == null) return null;
			var target = new AssemblyReferenceTarget(nameInfo, VirtualFileSystemPath.GetEmptyPathFor(InteractionContext.SolutionContext));
			var project = projectFile.GetProject().NotNull();
			return Resolve(target, project, resolveContext);
		}
	}
}
