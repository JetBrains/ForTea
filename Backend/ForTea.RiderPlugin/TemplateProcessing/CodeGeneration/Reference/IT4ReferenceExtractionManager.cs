using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference
{
	public interface IT4ReferenceExtractionManager
	{
		[NotNull, ItemNotNull]
		IEnumerable<MetadataReference> ExtractPortableReferencesTransitive(
			Lifetime lifetime,
			[NotNull] IT4File file);

		[NotNull]
		IEnumerable<T4AssemblyReferenceInfo> ExtractReferenceLocationsTransitive([NotNull] IT4File file);
	}
}
