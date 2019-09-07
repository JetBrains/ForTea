using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Properties
{
	public class T4PreprocessResultProjectFileProperties : IPsiSourceFileProperties
	{
		private T4PreprocessResultProjectFileProperties()
		{
		}

		public static T4PreprocessResultProjectFileProperties Instance { get; } =
			new T4PreprocessResultProjectFileProperties();

		public bool ShouldBuildPsi { get; } = false;
		public bool IsGeneratedFile { get; } = true;
		public bool IsICacheParticipant { get; } = false;
		public bool ProvidesCodeModel { get; } = false;
		public bool IsNonUserFile { get; } = true;
		public IEnumerable<string> GetPreImportedNamespaces() => Enumerable.Empty<string>();
		public string GetDefaultNamespace() => "";
		public ICollection<PreProcessingDirective> GetDefines() => EmptyArray<PreProcessingDirective>.Instance;
	}
}
