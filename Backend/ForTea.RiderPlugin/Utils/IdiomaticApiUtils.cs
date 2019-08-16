using System;
using JetBrains.Annotations;

namespace JetBrains.ForTea.RiderPlugin.Utils
{
	public static class IdiomaticApiUtils
	{
		public static U Let<T, U>([NotNull] this T receiver, [NotNull] Func<T, U> converter) => converter(receiver);
	}
}
