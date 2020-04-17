using System;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;

namespace GammaJul.ForTea.Core.Utils
{
	public static class IShellLocksExtensions
	{
		public static void QueueOrExecute(
			[NotNull] this IShellLocks locks,
			Lifetime lifetime,
			[NotNull] string name,
			[NotNull] Action action
		)
		{
			if (locks.Dispatcher.IsAsyncBehaviorProhibited) action();
			else locks.Queue(lifetime, name, action);
		}
	}
}
