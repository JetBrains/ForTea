using System;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
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
	public sealed class T4AssemblyReferenceResolver : IT4AssemblyReferenceResolver
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
		private static AssemblyReferenceTarget FindAssemblyReferenceTarget(string assemblyNameOrFile)
		{
			// assembly path
			var path = FileSystemPath.TryParse(assemblyNameOrFile);
			if (!path.IsEmpty && path.IsAbsolute) return path.ToAssemblyReferenceTarget();

			// assembly name
			var nameInfo = AssemblyNameInfo.TryParse(assemblyNameOrFile);
			if (nameInfo == null) return null;
			return nameInfo.ToAssemblyReferenceTarget();
		}

		[CanBeNull]
		private FileSystemPath Resolve(
			AssemblyReferenceTarget target,
			IProject project,
			IModuleReferenceResolveContext resolveContext
		) => ResolveManager.Resolve(target, project, resolveContext);

		public FileSystemPath Resolve(IT4AssemblyDirective directive) => Resolve(directive.Path);

		public FileSystemPath Resolve(string assemblyNameOrFile, IPsiSourceFile sourceFile)
		{
			var projectFile = sourceFile.ToProjectFile();
			if (projectFile == null) return null;
			var pathWithMacros = new T4PathWithMacros(assemblyNameOrFile, sourceFile, projectFile);
			return Resolve(pathWithMacros);
		}

		public FileSystemPath Resolve([NotNull] IT4PathWithMacros pathWithMacros) =>
			ResolveAsAbsolutePath(pathWithMacros)
			?? ResolveAsLightReference(pathWithMacros)
			?? ResolveAsAssemblyName(pathWithMacros)
			?? ResolveAsAssemblyFile(pathWithMacros);

		[CanBeNull]
		private static FileSystemPath ResolveAsAbsolutePath([NotNull] IT4PathWithMacros pathWithMacros)
		{
			var path = pathWithMacros.ResolvePath();
			if (path.IsAbsolute) return path;
			return null;
		}

		[CanBeNull]
		private FileSystemPath ResolveAsLightReference([NotNull] IT4PathWithMacros pathWithMacros)
		{
			string resolved = pathWithMacros.ResolveString();
			var lightResolved = LightWeightResolver.TryResolve(pathWithMacros.ProjectFile, resolved);
			if (lightResolved != null) return lightResolved;
			return null;
		}

		[CanBeNull]
		private FileSystemPath ResolveAsAssemblyName([NotNull] IT4PathWithMacros pathWithMacros)
		{
			string assemblyName = pathWithMacros.ResolveString();
			return ResolveAssemblyNameOrFile(pathWithMacros.ProjectFile, assemblyName);
		}

		[CanBeNull]
		private FileSystemPath ResolveAsAssemblyFile([NotNull] IT4PathWithMacros pathWithMacros)
		{
			string name = pathWithMacros.ResolveString();
			if (!name.EndsWith(".dll", StringComparison.Ordinal)) return null;
			string fileName = name.Substring(0, name.Length - 4);
			return ResolveAssemblyNameOrFile(pathWithMacros.ProjectFile, fileName);
		}

		[CanBeNull]
		private FileSystemPath ResolveAssemblyNameOrFile(
			[NotNull] IProjectFile projectFile,
			[NotNull] string assemblyNameOrFile
		)
		{
			var resolveContext = projectFile.SelectResolveContext();
			var target = FindAssemblyReferenceTarget(assemblyNameOrFile);
			if (target == null) return null;
			var project = projectFile.GetProject().NotNull();
			return Resolve(target, project, resolveContext);
		}
	}
}
