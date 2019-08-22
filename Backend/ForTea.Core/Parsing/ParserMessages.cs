namespace GammaJul.ForTea.Core.Parsing
{
	public static class ParserMessages
	{
		public const string IDS_CODE_BLOCK = "code block";
		public const string IDS_DIRECTIVE = "directive";
		public const string IDS_BLOCK = "block";

		public static string GetString(string id) => id;

		public static string GetUnexpectedTokenMessage() => "Unexpected token";

		public static string GetExpectedMessage(string expectedSymbol) =>
			GetUnexpectedTokenMessage();
	}
}
