using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	public interface IT4ModelInteractionHelper
	{
		Func<T4FileLocation, T> Wrap<T>([NotNull] Func<IT4File, T> wrappee, [NotNull] T defaultValue) where T : class;
		Action<T4FileLocation> Wrap([NotNull] Action<IT4File> wrappee);
	}
}
