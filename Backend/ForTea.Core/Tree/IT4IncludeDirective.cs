using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Tree
{
	public partial interface IT4IncludeDirective : IT4DirectiveWithPath
	{
		bool Once { get; }

		[CanBeNull]
		IT4IncludedFile IncludedFile { get; }
	}
}
