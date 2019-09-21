using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference
{
	[DebuggerDisplay("{" + nameof(FullName) + "}")]
	public readonly struct T4AssemblyReferenceInfo
	{
		[NotNull]
		public string FullName { get; }

		[NotNull]
		public FileSystemPath Location { get; }

		public T4AssemblyReferenceInfo([NotNull] string fullName, [NotNull] FileSystemPath location)
		{
			FullName = fullName;
			Location = location;
		}
	}
}
