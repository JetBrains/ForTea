using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi
{
	[SolutionComponent]
	public class T4TreeNavigator
	{
		private T4DirectiveInfoManager Manager { get; }
		public T4TreeNavigator(T4DirectiveInfoManager manager) => Manager = manager;

		[CanBeNull]
		public IT4AttributeValue FindIncludeValue([NotNull] IT4Include include)
		{
			var directive = include.PrevSibling as IT4Directive;
			Assertion.Assert(directive.IsSpecificDirective(Manager.Include), "unexpected include position");
			var attribute = directive.GetAttribute(Manager.Include.FileAttribute.Name);
			var value = attribute?.GetValueToken();
			return value;
		}

		[NotNull, ItemNotNull]
		public IEnumerable<IErrorElement> GetErrorElements([NotNull] ITreeNode origin)
		{
			for (var current = origin.LastChild; current != null; current = current.LastChild)
			{
				if (!(current is IErrorElement errorElement)) continue;
				yield return errorElement;
				current = errorElement.PrevSibling;
				if (current == null) break;
			}
		}

	}
}
