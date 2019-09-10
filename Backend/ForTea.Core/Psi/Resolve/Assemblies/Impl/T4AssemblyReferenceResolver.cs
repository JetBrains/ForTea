using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies.Impl
{
	[SolutionComponent]
	public sealed class T4AssemblyReferenceResolver : IT4AssemblyReferenceResolver
	{
		[NotNull]
		private IModuleReferenceResolveManager ResolveManager { get; }

		[NotNull]
		private ILogger Logger { get; }

		public T4AssemblyReferenceResolver(
			[NotNull] IModuleReferenceResolveManager resolveManager,
			[NotNull] ILogger logger
		)
		{
			ResolveManager = resolveManager;
			Logger = logger;
		}

		public AssemblyReferenceTarget FindAssemblyReferenceTarget(string assemblyNameOrFile)
		{
			// assembly path
			var path = FileSystemPath.TryParse(assemblyNameOrFile);
			if (!path.IsEmpty && path.IsAbsolute) return new AssemblyReferenceTarget(AssemblyNameInfo.Empty, path);

			// assembly name
			var nameInfo = AssemblyNameInfo.TryParse(assemblyNameOrFile);
			if (nameInfo == null) return null;
			return new AssemblyReferenceTarget(nameInfo, FileSystemPath.Empty);
		}

		public FileSystemPath Resolve(
			AssemblyReferenceTarget target,
			IProject project,
			IModuleReferenceResolveContext resolveContext
		)
		{
			using (new ILoggerStructuredEx.PhaseCookieDisposable(Logger.Verbose(), target.Name, "Resolution time"))
			{
				return ResolveManager.Resolve(target, project, resolveContext);
			}
		}
	}
}
