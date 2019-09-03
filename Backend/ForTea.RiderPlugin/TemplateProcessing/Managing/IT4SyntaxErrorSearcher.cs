using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4SyntaxErrorSearcher
	{
		[CanBeNull]
		ITreeNode FindErrorElement([NotNull] IT4File file);
	}
}
