using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Daemon.Syntax
{
	public static class T4SyntaxHighlightingUtils
	{
		[NotNull]
		public static string WithPrefix([NotNull] this string s, [NotNull] string prefix) => $"({prefix}) {s}";
	}
}
