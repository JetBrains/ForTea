using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi
{
	public static class T4TreeNavigator
	{
		[NotNull, ItemNotNull]
		public static IEnumerable<IErrorElement> GetErrorElements([NotNull] ITreeNode origin)
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
