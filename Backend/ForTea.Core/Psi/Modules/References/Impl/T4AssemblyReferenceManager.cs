using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules.References.Impl
{
	public sealed class T4AssemblyReferenceManager : IT4AssemblyReferenceManager
	{
		[NotNull]
		private IT4AssemblyReferenceResolver AssemblyReferenceResolver { get; }
		
		[NotNull]
		private IT4ProjectReferenceResolver ProjectReferenceResolver { get; }

		[NotNull]
		private IShellLocks ShellLocks { get; }

		[NotNull]
		private IDictionary<FileSystemPath, IAssemblyCookie> MyAssemblyReferences { get; }

		private IDictionary<FileSystemPath, IProject> MyProjectReferences { get; }

		public IEnumerable<IModule> AssemblyReferences =>
			MyAssemblyReferences.Values.SelectNotNull(cookie => cookie.Assembly);

		public IEnumerable<IModule> ProjectReferences => MyProjectReferences.Values;

		public IEnumerable<FileSystemPath> RawReferences => MyAssemblyReferences.Keys.Concat(MyProjectReferences.Keys);

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
			MyProjectReferences = new Dictionary<FileSystemPath, IProject>();
			MyAssemblyReferences = new Dictionary<FileSystemPath, IAssemblyCookie>();
			AssemblyFactory = assemblyFactory;
			File = file;
			ResolveContext = resolveContext;
			ShellLocks = shellLocks;
			var solution = File.GetSolution();
			AssemblyReferenceResolver = solution.GetComponent<IT4AssemblyReferenceResolver>();
			ProjectReferenceResolver = solution.GetComponent<IT4ProjectReferenceResolver>();
		}

		public bool TryRemoveReference(IT4PathWithMacros pathWithMacros)
		{
			var path = AssemblyReferenceResolver.Resolve(pathWithMacros);
			if (path == null) return false;
			if (MyAssemblyReferences.TryGetValue(path, out var cookie))
			{
				MyAssemblyReferences.Remove(path);
				cookie.Dispose();
				return true;
			}

			if (MyProjectReferences.ContainsKey(path))
			{
				MyProjectReferences.Remove(path);
				return true;
			}

			return false;
		}

		public bool TryAddReference(IT4PathWithMacros pathWithMacros)
		{
			var path = AssemblyReferenceResolver.Resolve(pathWithMacros);
			if (path == null) return false;
			return TryAddProjectReference(path) || TryAddAssemblyReference(path);
		}

		private bool TryAddProjectReference([NotNull] FileSystemPath path)
		{
			var project = ProjectReferenceResolver.TryResolveProject(path);
			if (project == null) return false;
			MyProjectReferences.Add(path, project);
			return true;
		}

		private bool TryAddAssemblyReference([NotNull] FileSystemPath path)
		{
			var cookie = AssemblyFactory.AddRef(path, "T4", ResolveContext);
			if (cookie != null) MyAssemblyReferences.Add(path, cookie);
			return true;
		}

		public void Dispose()
		{
			var assemblyCookies = MyAssemblyReferences.Values.ToArray();
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
			MyAssemblyReferences.Clear();
		}
	}
}
