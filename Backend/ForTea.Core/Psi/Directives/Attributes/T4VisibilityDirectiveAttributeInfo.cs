namespace GammaJul.ForTea.Core.Psi.Directives.Attributes
{
	public sealed class T4VisibilityDirectiveAttributeInfo : EnumDirectiveAttributeInfo
	{
		public const string Internal = "internal";
		public const string Public = "public";

		public T4VisibilityDirectiveAttributeInfo(DirectiveAttributeOptions options) :
			base("visibility", options, Public, Internal)
		{
		}
	}
}
