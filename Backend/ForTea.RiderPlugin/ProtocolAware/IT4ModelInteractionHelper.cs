using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.ReSharper.Psi;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	public interface IT4ModelInteractionHelper
	{
		[NotNull]
		Func<T4FileLocation, T> Wrap<T>([NotNull] Func<IT4File, T> wrappee, [NotNull] T defaultValue) where T : class;

		[NotNull]
		Func<T4FileLocation, T> Wrap<T>([NotNull] Func<IPsiSourceFile, T> wrappee, [NotNull] T defaultValue)
			where T : class;

		[NotNull]
		Func<T4FileLocation, Unit> Wrap([NotNull] Action<IT4File> wrappee);
	}
}
