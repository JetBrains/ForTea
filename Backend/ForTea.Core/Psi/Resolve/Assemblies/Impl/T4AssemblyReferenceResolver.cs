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
		private FileSystemPath ResolveAsLightReference([NotNull] IT4PathWithMacros pathWithMacros) =>
			LightWeightResolver.TryResolve(pathWithMacros.ProjectFile, pathWithMacros.ResolveString());

		[CanBeNull]
		private FileSystemPath ResolveAsAssemblyName([NotNull] IT4PathWithMacros pathWithMacros) =>
			ResolveAssemblyNameOrFile(pathWithMacros.ProjectFile, pathWithMacros.ResolveString());

		[CanBeNull]
		private FileSystemPath ResolveAsAssemblyFile([NotNull] IT4PathWithMacros pathWithMacros)
		{
			string name = pathWithMacros.ResolveString();
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
		private FileSystemPath ResolveAssemblyNameOrFile(
			[NotNull] IProjectFile projectFile,
			[NotNull] string assemblyNameOrFile
		)
		{
			var resolveContext = projectFile.SelectResolveContext();
			var nameInfo = AssemblyNameInfo.TryParse(assemblyNameOrFile);
			if (nameInfo == null) return null;
			var target = new AssemblyReferenceTarget(nameInfo, FileSystemPath.Empty);
			var project = projectFile.GetProject().NotNull();
			return Resolve(target, project, resolveContext);
		}
	}
}
