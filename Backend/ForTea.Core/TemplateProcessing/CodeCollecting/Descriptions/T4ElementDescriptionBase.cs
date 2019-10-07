using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public abstract class T4ElementDescriptionBase
	{
		[CanBeNull]
		private IPsiSourceFile SourceFile { get; }

		protected T4ElementDescriptionBase([CanBeNull] IPsiSourceFile source = null) => SourceFile = source;

		public bool HasSameSourceFile([CanBeNull] IPsiSourceFile file) => file == SourceFile;
	}
}
