using JetBrains.Annotations;

namespace GammaJul.ForTea.Core
{
	public static class T4FluentUtils
	{
		[CanBeNull]
		public static TTo As<TTo>([CanBeNull] this object subject)
		{
			if (!(subject is TTo)) return default;
			return (TTo) subject;
		}
	}
}
