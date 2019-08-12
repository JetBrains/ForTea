using System;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	public static class T4LockExtensions
	{
		public static void AssertReadAccessForbidden([NotNull] this IShellLocks locks)
		{
			if (locks == null) throw new ArgumentNullException(nameof(locks));
			Assertion.Assert(!locks.IsReadAccessAllowed(), "This action should not be done under read lock");
		}

		public static void AssertWriteAccessForbidden([NotNull] this IShellLocks locks)
		{
			if (locks == null) throw new ArgumentNullException(nameof(locks));
			Assertion.Assert(!locks.IsWriteAccessAllowed(), "This action should not be done under write lock");
		}
	}
}
