using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Reference
{
	public readonly struct T4AssemblyReferenceInfo
	{
		[NotNull]
		public string FullName { get; }

		[NotNull]
		public string Location { get; }

		public T4AssemblyReferenceInfo([NotNull] string fullName, [NotNull] string location)
		{
			FullName = fullName;
			Location = location;
		}
	}
}
