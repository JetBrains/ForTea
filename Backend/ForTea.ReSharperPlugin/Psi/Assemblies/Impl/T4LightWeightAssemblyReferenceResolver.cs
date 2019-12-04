using System;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using JetBrains.Annotations;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.Util;
using JetBrains.VsIntegration.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Assemblies.Impl
{
	[SolutionComponent]
	public sealed class T4LightWeightAssemblyReferenceResolver : IT4LightWeightAssemblyReferenceResolver
	{
		[NotNull] private readonly Lazy<Optional<ITextTemplatingComponents>> _components;

		[NotNull]
		private Optional<ITextTemplatingComponents> Components => _components.Value;

		public T4LightWeightAssemblyReferenceResolver([NotNull] RawVsServiceProvider provider) =>
			_components = Lazy.Of(() =>
					new Optional<ITextTemplatingComponents>(provider.Value.GetService<STextTemplating, ITextTemplatingComponents>()),
				true);

		public FileSystemPath TryResolve(IProjectFile file, string assemblyName)
		{
			using var _ = Prepare(file);
			string resolved = Components.CanBeNull?.Host?.ResolveAssemblyReference(assemblyName);
			if (resolved == null) return null;
			var path = FileSystemPath.Parse(resolved);
			if (path.IsAbsolute) return path;
			return null;
		}

		private IDisposable Prepare(IProjectFile file)
		{
			IVsHierarchy hierarchy = Utils.TryGetVsHierarchy(file);
			ITextTemplatingComponents components = Components.CanBeNull;

			if (components == null)
				return Disposable.Empty;

			object oldHierarchy = components.Hierarchy;
			string oldInputFileName = components.InputFile;

			return Disposable.CreateBracket(
				() =>
				{
					components.Hierarchy = hierarchy;
					components.InputFile = file.Location.IsNullOrEmpty() ? null : file.Location.FullPath;
				},
				() =>
				{
					components.Hierarchy = oldHierarchy;
					components.InputFile = oldInputFileName;
				},
				false
			);
		}
	}
}
