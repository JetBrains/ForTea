using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	public readonly struct T4TemplateExecutionManagerInfo
	{
		[NotNull]
		public string Code { get; }

		[NotNull]
		public IEnumerable<T4MetadataReferenceInfo> References { get; }

		[NotNull]
		public IT4File File { get; }

		public T4TemplateExecutionManagerInfo(
			[NotNull] string code,
			[NotNull] IEnumerable<T4MetadataReferenceInfo> references,
			[NotNull] IT4File file
		)
		{
			Code = code;
			References = references;
			File = file;
		}
	}
}
