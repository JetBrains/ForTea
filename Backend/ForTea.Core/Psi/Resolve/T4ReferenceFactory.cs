using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
	public sealed class T4ReferenceFactory : IReferenceFactory
	{
		[NotNull]
		private IT4IncludeResolver IncludeResolver { get; }

		public T4ReferenceFactory([NotNull] IT4IncludeResolver includeResolver) => IncludeResolver = includeResolver;

		public ReferenceCollection GetReferences(ITreeNode element, ReferenceCollection oldReferences)
		{
			if (!element.Language.Is<T4Language>()) return ReferenceCollection.Empty;
			if (!(element is IT4IncludeDirective directive)) return ReferenceCollection.Empty;
			var path = IncludeResolver.ResolvePath(directive.ResolvedPath);
			if (path.IsEmpty) return ReferenceCollection.Empty;
			var attribute = directive.GetFirstAttribute(T4DirectiveInfoManager.Include.FileAttribute);
			if (attribute == null) return ReferenceCollection.Empty;
			return new ReferenceCollection(new T4FileReference(attribute, attribute.Value, path));
		}

		public bool HasReference(ITreeNode element, IReferenceNameContainer names) =>
			!GetReferences(element, ReferenceCollection.Empty).IsEmpty;
	}
}
