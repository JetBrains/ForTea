using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.DataStructures;

namespace GammaJul.ForTea.Core.Psi.Directives.Attributes {

	public class EnumDirectiveAttributeInfo : DirectiveAttributeInfo {

		[NotNull] [ItemNotNull] private readonly JetImmutableArray<string> _enumValues;
		
		public override bool IsValid(string value)
			=> _enumValues.Contains(value, StringComparer.OrdinalIgnoreCase);

		public override JetImmutableArray<string> IntelliSenseValues
			=> _enumValues;

		public EnumDirectiveAttributeInfo(
			[NotNull] string name,
			DirectiveAttributeOptions options,
			[NotNull] [ItemNotNull] params string[] enumValues
		)
			: base(name, options) {
			_enumValues = enumValues.ToImmutableArray();
		}

	}

}