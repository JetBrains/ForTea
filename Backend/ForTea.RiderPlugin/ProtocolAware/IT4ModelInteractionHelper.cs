using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware
{
	public interface IT4ModelInteractionHelper
	{
		Func<T4FileLocation, T> Wrap<T>(Func<IT4File, T> wrappee, [NotNull] T defaultValue) where T : class;
		Func<T4FileLocation, T> WrapStructFunc<T>(Func<IT4File, T?> wrappee, T defaultValue) where T : struct;
	}
}
