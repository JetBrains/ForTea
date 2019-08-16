using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
	public class T4ReferenceFactory : IReferenceFactory
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		public T4ReferenceFactory([NotNull] T4DirectiveInfoManager manager) => Manager = manager;

		public ReferenceCollection GetReferences(ITreeNode element, ReferenceCollection oldReferences)
		{
			if (!element.Language.Is<T4Language>()) return ReferenceCollection.Empty;
			if (!(element is IT4AttributeValue value)) return ReferenceCollection.Empty;
			Assertion.Assert(element.Parent is IT4DirectiveAttribute, "element.Parent is IT4DirectiveAttribute");
			var attribute = (IT4DirectiveAttribute) element.Parent;
			Assertion.Assert(attribute.Parent is IT4Directive, "attribute.Parent is IT4Directive");
			var directive = (IT4Directive) attribute.Parent;
			if (!directive.IsSpecificDirective(Manager.Include)) return ReferenceCollection.Empty;
			Assertion.Assert(directive.NextSibling is IT4IncludeDirective, "directive.NextSibling is IT4Include");
			var include = (IT4IncludeDirective) directive.NextSibling;
			var path = include.Path;
			var resolvedPath = path.ResolvePath();
			if (path.IsEmpty) return ReferenceCollection.Empty;
			return new ReferenceCollection(new T4FileReference(attribute, value, resolvedPath));
		}

		public bool HasReference(ITreeNode element, IReferenceNameContainer names) =>
			!GetReferences(element, ReferenceCollection.Empty).IsEmpty;
	}
}
