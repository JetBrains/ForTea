using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference
{
	public interface IT4ReferenceExtractionManager
	{
		[NotNull, ItemNotNull]
		IEnumerable<PortableExecutableReference> ExtractPortableReferencesTransitive([NotNull] IT4File file, Lifetime lifetime);

		[NotNull]
		IEnumerable<T4AssemblyReferenceInfo> ExtractReferenceLocationsTransitive([NotNull] IT4File file);

		[NotNull, ItemNotNull]
		IEnumerable<IProject> GetProjectDependencies([NotNull] IT4File file);
	}
}
