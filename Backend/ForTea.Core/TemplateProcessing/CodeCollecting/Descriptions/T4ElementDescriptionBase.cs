using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public abstract class T4ElementDescriptionBase
	{
		[CanBeNull]
		private IT4File Origin { get; }

		protected T4ElementDescriptionBase([CanBeNull] IT4File source = null) => Origin = source;

		public bool HasSameSource([NotNull] IT4File file) => file == Origin;
	}
}
