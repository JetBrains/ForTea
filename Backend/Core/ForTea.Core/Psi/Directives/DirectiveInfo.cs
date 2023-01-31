using System;
using System.Collections.Immutable;
using System.Linq;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DataStructures;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Directives {

	/// <summary>Contains information about supported T4 directives.</summary>
	public abstract class DirectiveInfo {

		[NotNull]
		public string Name { get; }

		[NotNull]
		[ItemNotNull]
		public abstract ImmutableArray<DirectiveAttributeInfo> SupportedAttributes { get; }

		[CanBeNull]
		public DirectiveAttributeInfo GetAttributeByName([CanBeNull] string attributeName)
			=> String.IsNullOrEmpty(attributeName)
				? null
				: SupportedAttributes.FirstOrDefault(di => di.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));

		[NotNull]
		public IT4Directive CreateDirective([CanBeNull] params Pair<string, string>[] attributes)
			=> T4ElementFactory.CreateDirective(Name, attributes);

		protected DirectiveInfo([NotNull] string name) {
			Name = name;
		}

	}

}