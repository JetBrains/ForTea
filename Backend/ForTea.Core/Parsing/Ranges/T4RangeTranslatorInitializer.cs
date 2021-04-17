using System.Linq;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
	internal sealed class T4RangeTranslatorInitializer
	{
		internal void SetUpRangeTranslators([NotNull] File file)
		{
			file.DocumentRangeTranslator = new T4DocumentRangeTranslator(file);
			foreach (var include in file.Includes.Cast<IncludedFile>())
			{
				SetUpRangeTranslators(include);
			}
		}

		private void SetUpRangeTranslators([NotNull] IncludedFile file)
		{
			file.DocumentRangeTranslator = new T4DocumentRangeTranslator(file);
			foreach (var include in file.IncludedFilesEnumerable.Cast<IncludedFile>())
			{
				SetUpRangeTranslators(include);
			}
		}
	}
}
