using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using JetBrains.Annotations;
using JetBrains.DataStructures;

namespace GammaJul.ForTea.Core.Psi.Directives
{
	public class TemplateDirectiveInfo : DirectiveInfo
	{
		[NotNull]
		public DirectiveAttributeInfo CompilerOptionsAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo CultureAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo DebugAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo HostSpecificAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo InheritsAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo LanguageAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo LinePragmasAttribute { get; }

		[NotNull]
		public DirectiveAttributeInfo VisibilityAttribute { get; }

		public override ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		public TemplateDirectiveInfo() : base("template")
		{
			LanguageAttribute = new LanguageAttributeInfo();
			HostSpecificAttribute = new EnumDirectiveAttributeInfo(
				"hostspecific",
				DirectiveAttributeOptions.None,
				"true",
				"false",
				"trueFromBase"
			);
			DebugAttribute = new BooleanDirectiveAttributeInfo("debug", DirectiveAttributeOptions.None);
			InheritsAttribute = new DirectiveAttributeInfo("inherits", DirectiveAttributeOptions.None);
			CultureAttribute = new CultureDirectiveAttributeInfo("culture", DirectiveAttributeOptions.None);
			CompilerOptionsAttribute = new DirectiveAttributeInfo("compilerOptions", DirectiveAttributeOptions.None);
			LinePragmasAttribute = new BooleanDirectiveAttributeInfo("linePragmas", DirectiveAttributeOptions.None);
			VisibilityAttribute = new T4VisibilityDirectiveAttributeInfo(DirectiveAttributeOptions.None);


			SupportedAttributes = new List<DirectiveAttributeInfo>(8)
			{
				LanguageAttribute,
				HostSpecificAttribute,
				DebugAttribute,
				InheritsAttribute,
				CultureAttribute,
				CompilerOptionsAttribute,
				LinePragmasAttribute,
				VisibilityAttribute
			}.ToImmutableArray();
		}
	}
}
