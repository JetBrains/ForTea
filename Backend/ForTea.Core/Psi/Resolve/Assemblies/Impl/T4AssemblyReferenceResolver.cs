using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Infra;
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
		private AssemblyInfoDatabase AssemblyInfoDatabase { get; }

		[NotNull]
		private IT4AssemblyNamePreprocessor Preprocessor { get; }

		[NotNull]
		private IModuleReferenceResolveManager ResolveManager { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4AssemblyReferenceResolver([NotNull] IModuleReferenceResolveManager resolveManager,
			[NotNull] ILogger logger,
			[NotNull] IT4AssemblyNamePreprocessor preprocessor,
			[NotNull] AssemblyInfoDatabase assemblyInfoDatabase
		)
		{
			ResolveManager = resolveManager;
			Logger = logger;
			Preprocessor = preprocessor;
			AssemblyInfoDatabase = assemblyInfoDatabase;
		}

		public AssemblyReferenceTarget FindAssemblyReferenceTarget(string assemblyNameOrFile)
		{
			// assembly path
			var path = FileSystemPath.TryParse(assemblyNameOrFile);
			if (!path.IsEmpty && path.IsAbsolute) return path.ToAssemblyReferenceTarget();

			// assembly name
			var nameInfo = AssemblyNameInfo.TryParse(assemblyNameOrFile);
			if (nameInfo == null) return null;
			return nameInfo.ToAssemblyReferenceTarget();
		}

		public FileSystemPath Resolve(
			AssemblyReferenceTarget target,
			IProject project,
			IModuleReferenceResolveContext resolveContext
		) => ResolveManager.Resolve(target, project, resolveContext);

		public FileSystemPath Resolve(IT4AssemblyDirective directive)
		{
			var targetAttribute = T4DirectiveInfoManager.Assembly.NameAttribute;
			string assemblyNameOrFile = directive.GetFirstAttribute(targetAttribute)?.Value.GetText();
			if (assemblyNameOrFile == null) return null;
			var sourceFile = directive.GetSourceFile();
			if (sourceFile == null) return null;
			return Resolve(assemblyNameOrFile, sourceFile);
		}

		public FileSystemPath Resolve(string assemblyNameOrFile, IPsiSourceFile sourceFile)
		{
			var projectFile = sourceFile.ToProjectFile() ?? T4MacroResolveContextCookie.ProjectFile;
			if (projectFile == null) return null;
			using (Preprocessor.Prepare(projectFile))
			{
				string resolved = new T4PathWithMacros(assemblyNameOrFile, sourceFile).ResolveString();
				string path = Preprocessor.Preprocess(projectFile, resolved);
				var resolveContext = projectFile.SelectResolveContext();
				var target = FindAssemblyReferenceTarget(path);
				if (target == null) return null;
				return Resolve(target, projectFile.GetProject().NotNull(), resolveContext);
			}
		}

		public IEnumerable<T4AssemblyReferenceInfo> ResolveTransitiveDependencies(
			IEnumerable<T4AssemblyReferenceInfo> directDependencies,
			IModuleReferenceResolveContext resolveContext
		)
		{
			var result = new List<T4AssemblyReferenceInfo>();
			ResolveTransitiveDependencies(directDependencies, resolveContext, result);
			return result;
		}

		public IEnumerable<FileSystemPath> ResolveTransitiveDependencies(
			IList<FileSystemPath> directDependencies,
			IModuleReferenceResolveContext resolveContext
		)
		{
			return ResolveTransitiveDependencies(directDependencies.SelectMany(directDependency => AssemblyInfoDatabase
				.GetReferencedAssemblyNames(directDependency)
				.SelectNotNull<AssemblyNameInfo, T4AssemblyReferenceInfo>(assemblyNameInfo =>
				{
					var resolver = new AssemblyResolverOnFolders(directDependency.Parent);
					resolver.ResolveAssembly(assemblyNameInfo, out var path, resolveContext);
					if (path == null) return null;
					return new T4AssemblyReferenceInfo(assemblyNameInfo.FullName, path);
				})), resolveContext).Select(it => it.Location).Concat(directDependencies);
		}

		private void ResolveTransitiveDependencies(
			[NotNull] IEnumerable<T4AssemblyReferenceInfo> directDependencies,
			[NotNull] IModuleReferenceResolveContext resolveContext,
			[NotNull] IList<T4AssemblyReferenceInfo> destination
		)
		{
			foreach (var directDependency in directDependencies)
			{
				if (destination.Any(it => it.FullName == directDependency.FullName)) continue;
				destination.Add(directDependency);
				var indirectDependencies = AssemblyInfoDatabase
					.GetReferencedAssemblyNames(directDependency.Location)
					.SelectNotNull<AssemblyNameInfo, T4AssemblyReferenceInfo>(assemblyNameInfo =>
					{
						var resolver = new AssemblyResolverOnFolders(directDependency.Location.Parent);
						resolver.ResolveAssembly(assemblyNameInfo, out var path, resolveContext);
						if (path == null) return null;
						return new T4AssemblyReferenceInfo(assemblyNameInfo.FullName, path);
					});
				ResolveTransitiveDependencies(indirectDependencies, resolveContext, destination);
			}
		}
	}
}
