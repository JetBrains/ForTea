using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Psi.Resolve.Assemblies
{
	public interface IT4ProjectReferenceResolver
	{
		[NotNull, ItemNotNull]
		IEnumerable<IProject> GetProjectDependencies([NotNull] IT4File file);
	}
}
