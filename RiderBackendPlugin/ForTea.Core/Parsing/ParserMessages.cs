using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Parsing
{
	public static class ParserMessages
	{
		[NotNull] public const string IDS_CODE_BLOCK = "code block";
		[NotNull] public const string IDS_DIRECTIVE = "directive";
		[NotNull] public const string IDS_BLOCK = "block";

		[NotNull]
		public static string GetString(string id) => id;

		[NotNull]
		public static string GetUnexpectedTokenMessage() => "Unexpected token";

		[NotNull]
		public static string GetExpectedMessage(string expectedSymbol) =>
			string.Format(GetString("{0} expected"), expectedSymbol).Capitalize();
	}
}
