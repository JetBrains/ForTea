using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Psi.Modules.References;
using GammaJul.ForTea.Core.Psi.Resolve;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	[SolutionComponent]
	public sealed class T4AssemblyReferenceInvalidator
	{
		[NotNull]
		private IT4AssemblyNamePreprocessor AssemblyNamePreprocessor { get; }

		public T4AssemblyReferenceInvalidator([NotNull] IT4AssemblyNamePreprocessor preprocessor) =>
			AssemblyNamePreprocessor = preprocessor;

		public bool InvalidateAssemblies(
			[NotNull] T4DeclaredAssembliesDiff dataDiff,
			[NotNull] IProjectFile file,
			[NotNull] T4AssemblyReferenceManager referenceManager
		)
		{
			bool result = false;
			using (T4MacroResolveContextCookie.GetOrCreate(file))
			using (AssemblyNamePreprocessor.Prepare(file))
			{
				// removes the assembly references from the old assembly directives
				foreach (string assembly in dataDiff
					.RemovedAssemblies
					.Select(it => AssemblyNamePreprocessor.Preprocess(file, it.ResolveString()))
				)
				{
					bool assemblyExisted = referenceManager.References.TryGetValue(assembly, out var cookie);
					if (!assemblyExisted) continue;
					referenceManager.References.Remove(assembly);
					result = true;
					cookie.Dispose();
				}

				// adds assembly references from the new assembly directives
				foreach (var _ in dataDiff
					.AddedAssemblies
					.Select(it => AssemblyNamePreprocessor.Preprocess(file, it.ResolveString()))
					.Where(addedAssembly => !referenceManager.References.ContainsKey(addedAssembly))
					.Select(referenceManager.TryAddReference)
					.WhereNotNull()
				)
				{
					result = true;
				}
			}

			return result;
		}
	}
}
