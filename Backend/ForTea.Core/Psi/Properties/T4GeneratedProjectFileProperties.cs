using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Properties
{
	public sealed class T4GeneratedProjectFileProperties : IPsiSourceFileProperties
	{
		private T4GeneratedProjectFileProperties()
		{
		}

		public static T4GeneratedProjectFileProperties Instance { get; } = new T4GeneratedProjectFileProperties();
		public bool ShouldBuildPsi { get; } = true;
		public bool IsGeneratedFile { get; } = true;
		public bool IsICacheParticipant { get; } = true;
		public bool ProvidesCodeModel { get; } = true;
		public bool IsNonUserFile { get; } = true;
		public IEnumerable<string> GetPreImportedNamespaces() => Enumerable.Empty<string>();
		public string GetDefaultNamespace() => "";
		public ICollection<PreProcessingDirective> GetDefines() => EmptyArray<PreProcessingDirective>.Instance;
	}
}
