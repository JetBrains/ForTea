using Debugger.Common.MetadataAndPdb;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.Util;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	public struct T4MetadataReferenceInfo
	{
		[NotNull]
		public MetadataReference Reference { get; }

		[NotNull]
		public FileSystemPath Path { get; }

		public T4MetadataReferenceInfo([NotNull] MetadataReference reference, [NotNull] FileSystemPath path)
		{
			Reference = reference;
			Path = path;
		}

		public static T4MetadataReferenceInfo? FromPath(
			Lifetime lifetime,
			[NotNull] FileSystemPath path,
			[NotNull] RoslynMetadataReferenceCache cache
		)
		{
			var reference = cache.GetMetadataReference(lifetime, path);
			if (reference == null) return null;
			return new T4MetadataReferenceInfo(reference, path);
		}
	}
}
